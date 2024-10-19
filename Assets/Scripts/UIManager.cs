using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;
using Sequence = DG.Tweening.Sequence;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private GameObject squarePrefab;
    [SerializeField] private Transform squareParentTransform;
    [SerializeField] private TextAsset dataName;

    public Vector2 comboPos;
    public int comboCount;

    private List<Square> _squaresList = new();
    private BoardManager _boardManager;
    private Square _squareScript;
    private List<BoardAction> _actionsWrapList = new();
    private Sequence _sequence;
    public int idCount = 30;

    private const float MERGE_DURATION = 0.1f;
    private const float TIME_DELAY = 0.1f;



    private void Init()
    {
        Observer.On(Constants.EventKey.START_UI, e => StartUI(e));
        Observer.On(Constants.EventKey.RESET_GAME, e => ResetGame());
        Observer.On(Constants.EventKey.RESET_GAME_UI, e => ResetGameUI());
        Observer.On(Constants.EventKey.RENDER_UI, e => RenderUI(e));
        Observer.On(Constants.EventKey.SET_SCORE_UI, e => SetScoreUI());

        _boardManager = BoardManager.Instance;
        _squareScript = squarePrefab.GetComponent<Square>();
        GenerateStartChartScores();
    }

    public void ResetGameUI()
    {
        idCount = 30;
        for (var i = 0; i < _squaresList.Count; i++)
        {
            _squaresList[i].SetActiveObj(false);
        }
    }

    public void ResetGame()
    {
        _boardManager.RestartGame();
        ResetGameUI();
        SetScoreUI();
        _boardManager.isProcessing = false;
    }

    #region GenerateRank

    private void GenerateStartChartScores()
    {
        var rankData = JsonUtility.FromJson<Utils.RankData>(Prefs.RankData);

        if (rankData != null && DateTime.Now.Date.ToString(Constants.FomatText.SHORT_DATE_FORMAT).Equals(rankData.dateTimeString))
        {
            return;
        }

        var nameList = JsonUtility.FromJson<Utils.JsonHelper<string>>(dataName.text).data;

        List<Utils.ChartScore> chartScores = new();

        for (var i = 0; i < 19; i++)
        {
            var random = Random.Range(1000, 100 * 1000);
            chartScores.Add(new Utils.ChartScore(random, nameList[i]));
        }

        var dataSave = new Utils.RankData(chartScores, DateTime.Now.Date.ToString(Constants.FomatText.SHORT_DATE_FORMAT));
        Prefs.RankData = JsonUtility.ToJson(dataSave);
    }

    #endregion

    #region StartUI
    public void StartUI(object data)
    {
        Init();
        _squareScript = squarePrefab.GetComponent<Square>();
        idCount = _boardManager.idCount;

        InitPoolObject();
        ResetUI(data);
    }

    private void ResetUI(object data)
    {
        var squaresData = (List<SquareData>)data;
        foreach (var squareData in squaresData)
        {
            if (squareData.value <= 0)
            {
                continue;
            }

            var instanceNewSquareDataCommand = new InstanceNewSquareDataCommand(squareParentTransform, _squaresList, _squareScript, squareData.Position);
            var newSquareData = instanceNewSquareDataCommand.Excute();

            newSquareData.SetValue(squareData.value);
            newSquareData.SetId(squareData.id);
        }
    }
    #endregion

    private void InitPoolObject()
    {
        for (int i = 0; i < 30; i++)
        {
            var squarePool = Instantiate(_squareScript, Vector2.zero, Quaternion.identity, squareParentTransform);
            squarePool.SetActiveObj(false);
            _squaresList.Add(squarePool);
        }
    }

    public void ReturnPool(GameObject insObj)
    {
        insObj.SetActive(false);
    }

    #region RenderUI
    public void RenderUI(object data)
    {
        _actionsWrapList = (List<BoardAction>)data;

        if (_actionsWrapList.Count <= 0)
        {
            _boardManager.isProcessing = false;
            return;
        }

        InitNewSquareForShoot();
        comboCount = 0;
        _sequence = DOTween.Sequence();

        foreach (var actionListWrap in _actionsWrapList)
        {
            switch (actionListWrap.actionType)
            {
                case ActionType.Shoot:
                    new ShootUICommand(_squaresList, _sequence, actionListWrap.stepActionList[0], MERGE_DURATION, TIME_DELAY).Excute();
                    break;
                case ActionType.MergeAllBlock:
                    new MergeUICommand(_squaresList, _sequence, actionListWrap.stepActionList, MERGE_DURATION, TIME_DELAY).Excute();
                    break;
                case ActionType.SortAllBlock:
                    new SortUICommand(_squaresList, _sequence, actionListWrap.stepActionList, MERGE_DURATION, TIME_DELAY).Excute();
                    break;
                case ActionType.ClearMinBlock:
                    new ClearMinBlockUICommand(_squaresList, actionListWrap.stepActionList, _sequence).Excute();
                    break;
            }
        }

        _sequence.OnComplete(() =>
        {
            _boardManager.isProcessing = false;
            SetComboUI();

            if (_boardManager.isGameOver)
            {
                SetGameOverUI();
            }
        });
    }
    #endregion

    private void SetComboUI()
    {
        if (comboCount > 2)
        {
            Observer.Emit(Constants.EventKey.COMBO, new ComboEvent(comboCount, comboPos));
            Observer.Emit(Constants.EventKey.SOUND_COMBO);

            var endNewValueMerge = _actionsWrapList
                .Where(boardAction => boardAction.actionType == ActionType.MergeAllBlock)
                .Select(boardAction => boardAction.stepActionList[^1].newSquareValue)
                .Last();

            _boardManager.score += comboCount * endNewValueMerge;
            SetScoreUI();
        }
    }

    private void SetGameOverUI()
    {
        idCount = 30;
        _boardManager.isProcessing = true;
        Observer.Emit(Constants.EventKey.SOUND_OVER_GAME);
        Observer.Emit(Constants.EventKey.GAME_OVER_POPUP);
    }

    private void InitNewSquareForShoot()
    {
        idCount++;
        var newSquarePos = new Vector3(0, -10, 0);

        var instanceNewSquareDataCommand = new InstanceNewSquareDataCommand(squareParentTransform, _squaresList, _squareScript, newSquarePos);
        var squarePool = instanceNewSquareDataCommand.Excute();

        squarePool.SetId(idCount);
    }

    public void SetScoreUI()
    {
        _boardManager = BoardManager.Instance;
        if (_boardManager.score > _boardManager.highScore)
        {
            _boardManager.highScore = _boardManager.score;
        }

        Observer.Emit(Constants.EventKey.SCORE, new ScoreDataEvent(_boardManager.score, _boardManager.highScore));
    }

}