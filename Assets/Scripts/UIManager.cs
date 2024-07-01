using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UI;
using UnityEngine;
using uPools;
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
    private bool _isSave;
    private GameObjectPool _blockPool;
    private GamePlayScreen _gamePlayScreen;
    private SoundManager _soundManager;

    private const float MERGE_DURATION = 0.1f;
    private const float TIME_DELAY = 0.1f;

    // private static readonly ProfilerMarker ProcessingTweenMaker = new("MyMaker.DOTweenSequence");

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

    public void StartUI(List<SquareData> squaresData)
    {
        _squareScript = squarePrefab.GetComponent<Square>();
        InitPoolObject();
        ResetUI(squaresData);
    }

    private void InitPoolObject()
    {
        for (int i = 0; i < 30; i++)
        {
            var squarePool = Instantiate(_squareScript, Vector2.zero, Quaternion.identity, squareParentTransform);
            squarePool.SetActiveObj(false);
            _squaresList.Add(squarePool);
        }
    }

    private void ResetUI(List<SquareData> squaresData)
    {
        foreach (var squareData in squaresData)
        {
            if (squareData.value <= 0)
            {
                continue;
            }

            var newSquareData = InstanceNewSquareData(squareData.Position);

            newSquareData.SetValue(squareData.value);
            newSquareData.SetId(squareData.id);
        }
    }

    #region ResetUI

    private Square InstanceNewSquareData(Vector3 pos)
    {
        var squarePool = FindSquarePoolDeActive();
        if (squarePool is null)
        {
            squarePool = Instantiate(_squareScript, pos, Quaternion.identity, squareParentTransform);

            _squaresList.Add(squarePool);
        }
        else
        {
            squarePool.transform.position = pos;
            squarePool.gameObject.SetActive(true);
        }

        return squarePool;
    }

    private Square FindSquarePoolDeActive()
    {
        for (var i = _squaresList.Count - 1; i >= 0; i--)
        {
            if (!_squaresList[i].gameObject.activeSelf)
            {
                return _squaresList[i];
            }
        }

        return null;
    }

    #endregion

    public void ReturnPool(GameObject insObj)
    {
        insObj.SetActive(false);
    }

    public void RenderUI(List<BoardAction> actionsWrapList)
    {
        _actionsWrapList = actionsWrapList;
        
        // foreach (var actionListWrap in _actionsWrapList)
        // {
        //     Debug.Log("actionListWrap: " + JsonUtility.ToJson(actionListWrap));
        // }

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
                    ShootUI(_sequence, actionListWrap.stepActionList.First());
                    break;
                case ActionType.MergeAllBlock:
                    MergeUI(_sequence, actionListWrap.stepActionList);
                    break;
                case ActionType.SortAllBlock:
                    SortUI(_sequence, actionListWrap.stepActionList);
                    break;
                case ActionType.ClearMinBlock:
                    ClearMinBlockUI(_sequence, actionListWrap.stepActionList);
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

    #region RenderUI

    private void SetComboUI()
    {
        if (comboCount > 2)
        {
            _soundManager.PlaySoundComboSfx();
            _gamePlayScreen.ShowCombo();

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
        _gamePlayScreen.ShowGameOverPopup();
        _boardManager.isProcessing = true;
    }

    private void ShootUI(Sequence sequence, StepAction stepAction)
    {
        sequence.OnStart(_soundManager.PlaySoundShootSfx);
        var squarePool = FindSquarePoolById(stepAction.singleSquareSources.id);

        squarePool.SetValue(stepAction.newSquareValue);
        squarePool.transform.position = stepAction.singleSquareSources.Position;

        sequence.Append(squarePool.transform
            .DOMoveY(stepAction.squareTarget.Position.y, MERGE_DURATION)
            .SetEase(Ease.Linear));

        sequence.AppendInterval(TIME_DELAY);
    }

    private Square FindSquarePoolById(int id)
    {
        for (var i = _squaresList.Count - 1; i >= 0; i--)
        {
            if (_squaresList[i].squareData.id == id)
            {
                return _squaresList[i];
            }
        }

        return null;
    }

    private void InitNewSquareForShoot()
    {
        idCount++;
        var newSquarePos = new Vector3(0, -10, 0);

        var squarePool = InstanceNewSquareData(newSquarePos);

        squarePool.SetId(idCount);
    }

    private void MergeUI(Sequence sequence, List<StepAction> mergerActionList)
    {
        Sequence mergerSequence = DOTween.Sequence();
        comboCount++;

        mergerSequence.OnStart(_soundManager.PlaySoundMergeSfx);
        foreach (var mergerAction in mergerActionList)
        {
            var squareSourceGameObjectsList = FindAllSquareGameObjectsActiveSameValue(mergerAction);
            var squareTargetGameObject = FindSquarePoolById(mergerAction.squareTarget.id);

            foreach (var squareSourceGameObject in squareSourceGameObjectsList)
            {
                mergerSequence.Join(squareSourceGameObject.transform
                    .DOMove(mergerAction.squareTarget.Position, MERGE_DURATION)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        squareTargetGameObject.SetValue(mergerAction.newSquareValue);
                        squareSourceGameObject.SetValue(0);
                        squareSourceGameObject.ReturnPool();
                    })
                );
            }

            mergerSequence.OnComplete(() =>
            {
                comboPos = mergerAction.squareTarget.Position;
                _boardManager.score += mergerAction.newSquareValue;
                SetScoreUI();
            });
        }

        sequence.Append(mergerSequence);
        sequence.AppendInterval(TIME_DELAY);
    }

    private List<Square> FindAllSquareGameObjectsActiveSameValue(StepAction stepAction)
    {
        List<Square> list = new List<Square>();
        foreach (var squareGameObj in _squaresList)
        {
            if (IsCompareSquareActiveSameId(stepAction.multiSquareSources, squareGameObj))
            {
                list.Add(squareGameObj);
            }
        }

        return list;
    }

    private bool IsCompareSquareActiveSameId(List<SquareData> squareSources, Square squareGameObj)
    {
        var isSquareActiveSameIndex = false;
        foreach (var squareData in squareSources)
        {
            if (squareData.id == squareGameObj.squareData.id && squareGameObj.gameObject.activeSelf)
            {
                isSquareActiveSameIndex = true;
                break;
            }
        }

        return isSquareActiveSameIndex;
    }

    public void SetScoreUI(GamePlayScreen gamePlayScreen = null)
    {
        if (_boardManager.score > _boardManager.highScore)
        {
            _boardManager.highScore = _boardManager.score;
        }

        if (gamePlayScreen is not null)
        {
            _gamePlayScreen = gamePlayScreen;
        }

        if (_gamePlayScreen is not null)
        {
            _gamePlayScreen.SetScore();
        }
    }

    private void SortUI(Sequence sequence, List<StepAction> mergerActionList)
    {
        sequence.OnStart(_soundManager.PlaySoundSortSfx);
        Sequence sortSequence = DOTween.Sequence();

        foreach (var mergerAction in mergerActionList)
        {
            var squareSourceGameObject = FindSquarePoolById(mergerAction.singleSquareSources.id);

            sortSequence.Join(squareSourceGameObject.transform
                .DOMove(mergerAction.squareTarget.Position, MERGE_DURATION)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    var mergeEndPos = mergerAction.singleSquareSources.Position;
                    squareSourceGameObject.SetValue(mergerAction.newSquareValue);
                    if ((mergeEndPos - comboPos).sqrMagnitude == 0)
                    {
                        comboPos = mergerAction.squareTarget.Position;
                    }
                })
            );
        }

        sequence.Append(sortSequence);
        sequence.AppendInterval(TIME_DELAY);
    }

    private void ClearMinBlockUI(Sequence sequence, List<StepAction> stepActionList)
    {
        Sequence clearMinValueSequence = DOTween.Sequence();

        foreach (var squareGameObject in _squaresList)
        {
            foreach (var stepAction in stepActionList)
            {
                if (stepAction.squareTarget.id != squareGameObject.squareData.id)
                {
                    continue;
                }

                clearMinValueSequence.Join(squareGameObject.transform
                    .DOMove(stepAction.squareTarget.Position, 0)
                    .OnComplete(() =>
                    {
                        squareGameObject.SetValue(0);
                        squareGameObject.ReturnPool();
                    }));
            }
        }

        sequence.Append(clearMinValueSequence);
    }

    #endregion
}