using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.UI;
using uPools;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private GameObject squarePrefab;
    [SerializeField] private Transform squareParentTransform;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text highScoreText;
    [SerializeField] private GameObject comboPrefab;
    [SerializeField] private TextMesh comboText;
    [SerializeField] private GameObject gameOverPopup;

    private BoardManager _boardManager;
    private Square _squareScript;
    private List<Square> _squaresList = new();
    private List<BoardAction> _actionsWrapList = new();
    private Sequence _sequence;
    public int idCount = 30;
    private int _comboCount;
    private bool _isSave;
    private Vector2 _comboPos;
    private GameObjectPool blockPool;

    private const string FORMAT_SCORE = "0000";
    private const string COMBO_TEXT_FORMAT = "Combo x{0}";
    private const float MERGE_DURATION = 0.1f;
    private const float TIME_DELAY = 0.1f;

    private static readonly ProfilerMarker ProcessingTweenMaker = new("MyMaker.DOTweenSequence");

    private void Start()
    {
        _boardManager = BoardManager.Instance;

        comboPrefab.SetActive(false);
        gameOverPopup.SetActive(false);

        SetScoreUI();
    }

    public void StartUI(List<SquareData> squaresData)
    {
        _squareScript = squarePrefab.GetComponent<Square>();

        InitPoolObject();

        // TestData.SetDataTest(squaresData);

        ResetUI(squaresData);
    }

    private void InitPoolObject()
    {
        for (int i = 0; i < 30; i++)
        {
            var squarePool = Instantiate(_squareScript, Vector2.zero, Quaternion.identity, squareParentTransform);
            squarePool.SetActiveObj(false);
        }
    }

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

    public void ReturnPool(GameObject insObj)
    {
        insObj.SetActive(false);
    }

    public void RenderUI(List<BoardAction> actionsWrapList)
    {
        InitSquare();
        _comboCount = 0;
        _sequence = DOTween.Sequence();

        _actionsWrapList = actionsWrapList;

        // foreach (var actionListWrap in _actionsWrapList)
        // {
        //     Debug.Log(".....actionListWrap: " + JsonUtility.ToJson(actionListWrap));
        // }

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
        });

        if (_boardManager.isGameOver)
        {
            // Debug.Log("Game Over");
            SetGameOverUI();
        }
    }

    private void SetComboUI()
    {
        if (_comboCount > 2)
        {
            comboText.text = string.Format(COMBO_TEXT_FORMAT, _comboCount);
            comboPrefab.transform.position = new Vector2(_comboPos.x, _comboPos.y - 1.2f);
            comboPrefab.SetActive(true);

            var endNewValueMerge = _actionsWrapList
                .Where(boardAction => boardAction.actionType == ActionType.MergeAllBlock)
                .Select(boardAction => boardAction.stepActionList[^1].newSquareValue)
                .Last();

            _boardManager.score += _comboCount * endNewValueMerge;
            SetScoreUI();
            StartCoroutine(DeActiveComboIE());
        }
    }

    private void SetGameOverUI()
    {
        gameOverPopup.SetActive(true);
        _boardManager.isProcessing = true;
    }

    public void RePlayGame()
    {
        _boardManager.isProcessing = false;
        gameOverPopup.SetActive(false);
        _boardManager.RestartGame();
    }

    private IEnumerator DeActiveComboIE()
    {
        yield return new WaitForSeconds(1f);
        comboPrefab.SetActive(false);
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


    private void ShootUI(Sequence sequence, StepAction stepAction)
    {
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

    private void InitSquare()
    {
        idCount++;
        var newSquarePos = new Vector3(0, 6, 0);

        var squarePool = InstanceNewSquareData(newSquarePos);

        squarePool.SetId(idCount);
    }

    private void MergeUI(Sequence sequence, List<StepAction> mergerActionList)
    {
        Sequence mergerSequence = DOTween.Sequence();
        _comboCount++;

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
                _comboPos = mergerAction.squareTarget.Position;
                _boardManager.score += mergerAction.newSquareValue;
                SetScoreUI();
            });
        }

        sequence.Append(mergerSequence);
        sequence.AppendInterval(TIME_DELAY);
    }

    private void SetScoreUI()
    {
        scoreText.text = _boardManager.score.ToString(FORMAT_SCORE);
        if (_boardManager.score > _boardManager.highScore)
        {
            _boardManager.highScore = _boardManager.score;
        }

        highScoreText.text = _boardManager.highScore.ToString(FORMAT_SCORE);
    }

    private void SortUI(Sequence sequence, List<StepAction> mergerActionList)
    {
        Sequence sortSequence = DOTween.Sequence();

        foreach (var mergerAction in mergerActionList)
        {
            var squareSourceGameObject = FindSquarePoolById(mergerAction.singleSquareSources.id);

            sortSequence.Join(squareSourceGameObject.transform
                .DOMove(mergerAction.squareTarget.Position, MERGE_DURATION)
                .SetEase(Ease.Linear)
                .OnComplete(() => { squareSourceGameObject.SetValue(mergerAction.newSquareValue); })
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

    private List<Square> FindAllSquareGameObjectsActiveSameValue(StepAction stepAction)
    {
        return _squaresList.FindAll(squareGameObj =>
            IsCompareSquareActiveSameId(stepAction.multiSquareSources, squareGameObj));
    }

    private bool IsCompareSquareActiveSameId(List<SquareData> squareSources, Square squareGameObj)
    {
        var isSquareActiveSameIndex = squareSources.Any(squareData =>
            squareData.id == squareGameObj.squareData.id && squareGameObj.gameObject.activeSelf);
        return isSquareActiveSameIndex;
    }
}