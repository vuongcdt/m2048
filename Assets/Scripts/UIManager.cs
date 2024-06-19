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
    [SerializeField] private GameObject comboPrefab;
    [SerializeField] private TextMesh comboText;

    private BoardManager _boardManager;
    private const float MERGE_DURATION = 0.1f;
    private const float TIME_DELAY = 0.1f;
    private List<Square> _squaresList = new();
    private List<BoardAction> _actionsWrapList = new();
    private Sequence _sequence;
    public int _idCount = 30;
    private int _comboCount;
    private bool _isSave;
    private Vector2 _comboPos;
    private static readonly ProfilerMarker ProcessingTweenMaker = new("MyMaker.DOTweenSequence");

    private GameObjectPool blockPool;

    public void StartUI(List<SquareData> squaresData)
    {
        InitPoolObject();

        // squaresData[0].value = 2;
        // squaresData[6].value = 4;

        ////
        // squaresData[0].value = 16;
        // squaresData[1].value = 2;
        // squaresData[2].value = 16;
        //
        // squaresData[6].value = 2;
        // squaresData[8].value = 2;
        //
        // squaresData[12].value = 64;
        // // squaresData[18].value = 549755813888;
        // // squaresData[18].value = 8589934592;
        // // squaresData[18].value = 512 * 1024 * 1024 * 1024;
        //
        // ////
        // squaresData[4].value = 8;
        // squaresData[5].value = 2;
        // squaresData[11].value = 4;

        ////
        // squaresData[0].value = 2;
        // squaresData[1].value = 8;
        // squaresData[2].value = 2;
        //
        // squaresData[6].value = 8;
        // squaresData[7].value = 2;
        //
        // squaresData[12].value = 4;
        // squaresData[13].value = 8;
        //
        // squaresData[19].value = 4;

        ////
        // squaresData[0].value = 2;
        // squaresData[6].value = 4;
        // squaresData[12].value = 2;

        ////
        // squaresData[6].value = 8;
        // squaresData[7].value = 2;

        ResetUI(squaresData);
    }

    private void InitPoolObject()
    {
        // blockPool = new GameObjectPool(squarePrefab);
        // blockPool.Prewarm(30);
    }

    // private GameObject InstanceNewSquareData(Vector3 pos)
    // {
    //     return blockPool.Rent(pos, Quaternion.identity, squareParentTransform);
    // }

    private Square InstanceNewSquareData(Vector3 pos)
    {
        // return blockPool.Rent(pos, Quaternion.identity, squareParentTransform);
        var squarePool = _squaresList.Find(square => !square.gameObject.activeSelf);
        if (squarePool != null)
        {
            squarePool.transform.position = pos;
            squarePool.gameObject.SetActive(true);
        }
        else
        {
            squarePool = Instantiate(squarePrefab.GetComponent<Square>(), pos, Quaternion.identity, squareParentTransform);

            _squaresList.Add(squarePool);
        }

        return squarePool;
    }

    public void ReturnPool(GameObject insObj)
    {
        // blockPool.Return(insObj);
        insObj.SetActive(false);
    }
    
    public void RenderUI(List<BoardAction> actionsWrapList)
    {
        InitSquare();
        _comboCount = 0;
        _sequence = DOTween.Sequence();

        _actionsWrapList = actionsWrapList;

        foreach (var actionListWrap in _actionsWrapList)
        {
            Debug.Log(".....actionListWrap: " + JsonUtility.ToJson(actionListWrap));
        }

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
            if (_comboCount > 2)
            {
                comboText.text = $"Combo x{_comboCount}";
                comboPrefab.transform.position = new Vector2(_comboPos.x, _comboPos.y - 1.2f);
                comboPrefab.SetActive(true);

                StartCoroutine(DeActiveComboIE());
            }
        });
    }

    private IEnumerator DeActiveComboIE()
    {
        yield return new WaitForSeconds(1f);
        comboPrefab.SetActive(false);
    }

    private void Start()
    {
        _boardManager = BoardManager.Instance;
        comboPrefab.SetActive(false);
        SetScoreUI();
    }
    
    // private void ResetUI(List<SquareData> squaresData)
    // {
    //     squaresData
    //         .FindAll(squareData => squareData.value > 0)
    //         .ForEach(squareData =>
    //         {
    //             var newSquareData = InstanceNewSquareData(squareData.Position);
    //             var newSquareComp = newSquareData.GetComponent<Square>();
    //             newSquareComp.SetValue(squareData.value);
    //             newSquareComp.SetId(squareData.cell.Column + 1 + squareData.cell.Row * 6); // todo
    //             _squaresList.Add(newSquareComp);
    //         });
    // }
    
    private void ResetUI(List<SquareData> squaresData)
    {
        squaresData
            .FindAll(squareData => squareData.value > 0)
            .ForEach(squareData =>
            {
                var newSquareData = InstanceNewSquareData(squareData.Position);
                
                newSquareData.SetValue(squareData.value);
                newSquareData.SetId(squareData.id);
                // newSquareData.SetId(squareData.cell.Column + 1 + squareData.cell.Row * 6); // todo
            });
    }


    private void ShootUI(Sequence sequence, StepAction stepAction)
    {
        var squarePool = _squaresList.Find(squareGameObject =>
            squareGameObject.squareData.id == stepAction.singleSquareSources.id);

        squarePool.SetValue(stepAction.newSquareValue);
        squarePool.transform.position = stepAction.singleSquareSources.Position;

        sequence.Append(squarePool.transform
                .DOMoveY(stepAction.squareTarget.Position.y, MERGE_DURATION)
                .SetEase(Ease.Linear));
        sequence.AppendInterval(TIME_DELAY);
    }

    // private void InitSquare()
    // {
    //     _idCount++;
    //     var newSquarePos = new Vector3(0, 6, 0);
    //
    //     var squarePool = InstanceNewSquareData(newSquarePos);
    //     var newSquareComp = squarePool.GetComponent<Square>();
    //
    //     Debug.Log($"squarePool id {newSquareComp.squareData.id}");
    //
    //     newSquareComp.SetId(_idCount);
    //     Debug.Log($"newSquareComp id {newSquareComp.squareData.id}");
    //
    //     _squaresList.Add(newSquareComp);
    // }
    
    private void InitSquare()
    {
        _idCount++;
        var newSquarePos = new Vector3(0, 6, 0);

        var squarePool = InstanceNewSquareData(newSquarePos);

        Debug.Log($"squarePool id {squarePool.squareData.id}");

        squarePool.SetId(_idCount);
        Debug.Log($"squarePool id {squarePool.squareData.id}");
    }


    private void MergeUI(Sequence sequence, List<StepAction> mergerActionList)
    {
        Sequence mergerSequence = DOTween.Sequence();
        _comboCount++;

        foreach (var mergerAction in mergerActionList)
        {
            var squareSourceGameObjectsList = FindAllSquareGameObjectsActiveSameValue(mergerAction);
            var squareTargetGameObject = FindSquareGameObjectActiveById(mergerAction.squareTarget.id);

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
        scoreText.text = _boardManager.score.ToString();
        if (_boardManager.score > _boardManager.highScore)
        {
            _boardManager.highScore = _boardManager.score;
        }
    }

    private void SortUI(Sequence sequence, List<StepAction> mergerActionList)
    {
        Sequence sortSequence = DOTween.Sequence();

        foreach (var mergerAction in mergerActionList)
        {
            var squareSourceGameObject = FindSquareGameObjectActiveById(mergerAction.singleSquareSources.id);

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

    private Square FindSquareGameObjectActiveById(int id)
    {
        return _squaresList.Find(squareGameObj => squareGameObj.squareData.id == id);
    }

    private bool IsCompareSquareActiveSameId(List<SquareData> squareSources, Square squareGameObj)
    {
        var isSquareActiveSameIndex = squareSources.Any(squareData =>
            squareData.id == squareGameObj.squareData.id && squareGameObj.gameObject.activeSelf);
        return isSquareActiveSameIndex;
    }

    // #region SaveAndLoadGame
    //
    // private void OnApplicationQuit()
    // {
    //     SaveGame();
    // }
    //
    // private void OnApplicationPause(bool pauseStatus)
    // {
    //     CheckSaveGame(pauseStatus);
    // }
    //
    // private void OnApplicationFocus(bool hasFocus)
    // {
    //     CheckSaveGame(!hasFocus);
    // }
    //
    // private void CheckSaveGame(bool isPause)
    // {
    //     if (isPause)
    //     {
    //         SaveGame();
    //     }
    //
    //     if (!isPause)
    //     {
    //         _isSave = false;
    //     }
    // }
    //
    // private void SaveGame()
    // {
    //     if (_isSave)
    //     {
    //         return;
    //     }
    //
    //     _isSave = true;
    //
    //     var jsonHelper = new Utils.JsonHelper(_boardManager.squaresData);
    //
    //     Prefs.SquaresData = JsonUtility.ToJson(jsonHelper);
    //     Prefs.Score = _boardManager.score.ToString();
    //     Prefs.HighScore = _boardManager.highScore.ToString();
    //     Prefs.IdCount = _boardManager.idCount;
    //     Prefs.NextSquareValue = _boardManager.nextSquareValue.ToString();
    // }
    //
    // private void LoadDataFromPrefs()
    // {
    //     // _boardManager.squaresData = JsonUtility.FromJson<Utils.JsonHelper>(Prefs.SquaresData).datas;
    //     _boardManager.score = long.Parse(Prefs.Score);
    //     _boardManager.highScore = long.Parse(Prefs.HighScore);
    //     _boardManager.idCount = Prefs.IdCount;
    //     _boardManager.nextSquareValue = long.Parse(Prefs.NextSquareValue);
    //     Debug.Log($"_boardManager.score {_boardManager.score}");
    // }
    //
    // #endregion
}