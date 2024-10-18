using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UI;
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
    private SoundManager _soundManager;

    private const float MERGE_DURATION = 0.1f;
    private const float TIME_DELAY = 0.1f;

    private void Start()
    {
        _boardManager = BoardManager.Instance;
        _soundManager = SoundManager.Instance;

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
            var random = Random.Range(1000, 100000);
            chartScores.Add(new Utils.ChartScore(random, nameList[i]));
        }

        var dataSave = new Utils.RankData(chartScores, DateTime.Now.Date.ToString(Constants.FomatText.SHORT_DATE_FORMAT));
        Prefs.RankData = JsonUtility.ToJson(dataSave);
    }

    #endregion

    #region StartUI
    public void StartUI(List<SquareData> squaresData)
    {
        _squareScript = squarePrefab.GetComponent<Square>();
        InitPoolObject();
        // ResetUI(squaresData);
        var resetUICommand = new ResetUICommnad(squareParentTransform, _squaresList, _squareScript, squaresData);
        resetUICommand.Excute();
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
    public void RenderUI(List<BoardAction> actionsWrapList)
    {
        _actionsWrapList = actionsWrapList;

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


    private void SetComboUI()
    {
        if (comboCount > 2)
        {
            _soundManager.PlaySoundComboSfx();

            Observer.Emit(Constants.EventKey.COMBO, new ComboEvent(comboCount, comboPos));

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
        _soundManager.PlaySoundGameOverSfx();
        _boardManager.isProcessing = true;
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

    public void SetScoreUI(GamePlayScreen gamePlayScreen = null)
    {
        _boardManager = BoardManager.Instance;
        if (_boardManager.score > _boardManager.highScore)
        {
            _boardManager.highScore = _boardManager.score;
        }

        var data = new ScoreDataEvent()
        {
            highScore = _boardManager.highScore,
            score = _boardManager.score,
        };
        Observer.Emit(Constants.EventKey.SCORE, data);
    }

    #endregion
}