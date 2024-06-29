using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : Singleton<BoardManager>
{
    [SerializeField] private GameObject lineColumn;
    [SerializeField] private Transform lineParentTransform;

    public List<SquareData> squaresData = new();
    public int boardRow = 5;
    public int boardCol = 6;
    public bool isTouchLine;
    public int columnSelect;
    public bool isProcessing;
    public float score;
    public float highScore;
    public float nextSquareValue;
    public bool isClearData;
    public bool isGameOver;
    public bool isPlaying;

    private int idCount;
    private bool _isSave;
    private UIManager _uiManager;
    private SoundManager _soundManager;
    private SquareData _processingSquare;
    private List<GameObject> _lineColumnList = new();
    private List<StepAction> _actionsList = new();
    private List<BoardAction> _actionsWrapList = new();
    private List<float> _squareValueList = new() { 2 };

    // private List<int> _squareValueList = new() { 2, 4, 8, 16, 32, 64, 128 };
    private readonly int[] _probabilityList = { 1, 4, 10, 18, 28, 30, 44, 60, 78 };
    private GamePlayScreen _gamePlayScreen;

    private const int MAX_COUNT_QUARE_VALUE_LIST = 9;

    // private static readonly ProfilerMarker ProcessingDataMaker = new("MyMaker.ProcessingData");
    // private static readonly ProfilerMarker RenderUIMaker = new("MyMaker.RenderUI");

    private void Start()
    {
        Application.targetFrameRate = 60;
        _uiManager = UIManager.Instance;
        _soundManager = SoundManager.Instance;
        RenderLineColumn();

        if (isClearData)
        {
            RestartGame();
        }
        else
        {
            LoadDataFromPrefs();
        }

        // TestData.SetDataTest(squaresData);

        if (nextSquareValue == 0)
        {
            nextSquareValue = 2;
        }

        _uiManager.StartUI(squaresData);
        Debug.Log($"idCount1  {idCount}");
    }

    private void RenderLineColumn()
    {
        for (int i = 0; i < boardCol; i++)
        {
            var posLine = new Vector2(i * 2 - boardRow, 0);
            var line = Instantiate(lineColumn, posLine, Quaternion.identity, lineParentTransform);

            line.GetComponent<LineColumn>().column = i;

            _lineColumnList.Add(line);
        }
    }

    public void RestartGame()
    {
        idCount = 0;
        RenderBoard();
        score = 0;
        LoadHighScore();
        _squareValueList = new() { 2 };

        SetRandomSquareValue();
    }

    private void RenderBoard()
    {
        squaresData = new();
        for (var y = boardRow; y > 0; y--)
        {
            for (var x = 0; x < boardCol; x++)
            {
                idCount++;
                squaresData.Add(new SquareData(
                    new Utils.Cell(x, boardRow - y),
                    idCount,
                    0));
            }
        }
    }

    public IEnumerator ShootBlock()
    {
        isProcessing = true;
        _actionsWrapList.Clear();
        yield return new WaitForNextFrameUnit();

        // ProcessingDataMaker.Begin();
        ProcessingData(columnSelect);
        // ProcessingDataMaker.End();

        if (_actionsWrapList.Count <= 0)
        {
            yield return null;
        }

        yield return new WaitForNextFrameUnit();

        if (_actionsWrapList.Count > 0)
        {
            SetRandomSquareValue();
        }

        CheckGameOver();

        // RenderUIMaker.Begin();
        _uiManager.RenderUI(_actionsWrapList);
        // RenderUIMaker.End();
    }

    #region ShootBlock

    private void ProcessingData(int column)
    {
        Shoot(column);
        if (_actionsWrapList.Count <= 0)
        {
            _soundManager.PlaySoundMaxItemColumnSfx();
            return;
        }

        ProcessingLoop();
    }

    private void ProcessingLoop()
    {
        int countActionsList;
        do
        {
            countActionsList = _actionsWrapList.Count;

            MergeAllBlock();
            SortAllBlock();
        } while (countActionsList < _actionsWrapList.Count);
    }

    private void Shoot(int column)
    {
        _actionsList.Clear();
        var action = new StepAction();
        var squareTarget = GetEmptySquareDataTargetByColumn(column);
        if (squareTarget == null)
        {
            CheckMergeWhenMaxItemColumn(column, action);
            return;
        }

        Debug.Log($"idCount {idCount}");
        idCount++;
        var squareSource = new SquareData(
            new Utils.Cell(column, boardRow),
            idCount,
            nextSquareValue
        );

        action.squareTarget = new SquareData(squareTarget.cell, squareTarget.id, squareTarget.value);
        action.singleSquareSources = new SquareData(squareSource.cell, squareSource.id, squareSource.value);
        action.newSquareValue = nextSquareValue;

        _actionsList.Add(action);
        var item = new BoardAction(new List<StepAction>(_actionsList), ActionType.Shoot);
        _actionsWrapList.Add(item);

        squareTarget.id = squareSource.id;
        squareSource.id = 0;
        squareTarget.value = nextSquareValue;
        _processingSquare = squareTarget;
    }

    private void CheckMergeWhenMaxItemColumn(int column, StepAction action)
    {
        foreach (var squareTarget in squaresData)
        {
            if (Mathf.Approximately(squareTarget.value, nextSquareValue) &&
                squareTarget.cell.Column == column &&
                squareTarget.cell.Row == boardRow - 1)
            {
                idCount++;
                var squareSource = new SquareData(
                    new Utils.Cell(column, boardRow),
                    idCount,
                    nextSquareValue
                );

                var newSquareValue = nextSquareValue * 2;
                action.squareTarget = new SquareData(squareTarget.cell, squareTarget.id, squareTarget.value);
                action.multiSquareSources.Add(new SquareData(squareSource.cell, squareSource.id, squareSource.value));
                action.newSquareValue = newSquareValue;

                _actionsList.Add(action);
                var item = new BoardAction(new List<StepAction>(_actionsList), ActionType.MergeAllBlock);
                _actionsWrapList.Add(item);
                squareTarget.value = newSquareValue;
                _processingSquare = squareTarget;

                return;
            }
        }
    }

    private SquareData GetEmptySquareDataTargetByColumn(int column)
    {
        foreach (var block in squaresData)
        {
            if (block.value == 0 && block.cell.Column == column)
            {
                return block;
            }
        }

        return null;
    }

    private void CheckGameOver()
    {
        bool all = true;
        for (var index = 0; index < squaresData.Count; index++)
        {
            var squareData = squaresData[index];
            var isMaxItemColumnCanMerge = index >= 24 && Mathf.Approximately(squareData.value, nextSquareValue);
            if (squareData.value <= 0 || isMaxItemColumnCanMerge)
            {
                all = false;
                break;
            }
        }

        isGameOver = all;
        // Debug.Log($"isGameOver {isGameOver}");
    }

    #endregion

    private void MergeAllBlock()
    {
        _actionsList.Clear();

        var squareMergeOrderByCountSameValueList = GetSquareMergeOrderByCountSameValueList();

        CreateActionByMergeType(squareMergeOrderByCountSameValueList);

        if (_actionsList.Count > 0)
        {
            _actionsWrapList.Add(new BoardAction(new List<StepAction>(_actionsList),
                ActionType.MergeAllBlock));
        }
    }

    #region MergeBlock

    private void CreateActionByMergeType(List<Utils.CountSquareList> squareMergeOrderByCountSameValueList)
    {
        foreach (var data in squareMergeOrderByCountSameValueList)
        {
            if (data.squareSameValueList.Count == 1)
            {
                MergeSingleBlock(data.square, data.squareSameValueList[0]);
            }
            else
            {
                MergeMultiBlock(data.square, data.squareSameValueList);
            }
        }
    }

    private List<Utils.CountSquareList> GetSquareMergeOrderByCountSameValueList()
    {
        List<Utils.CountSquareList> squareMergeOrderByCountSameValueList = new();
        foreach (var block in squaresData)
        {
            if (!(block.value > 0))
            {
                continue;
            }

            List<SquareData> squareSameValueList = new();
            GetCountSquareSameValueList(block, squareSameValueList);

            // Debug.Log($"count {squareSameValueList.Count}");

            if (squareSameValueList.Count <= 0)
            {
                continue;
            }

            squareMergeOrderByCountSameValueList.Add(new Utils.CountSquareList(block, squareSameValueList));
        }

        squareMergeOrderByCountSameValueList
            .Sort((a, b) => b.squareSameValueList.Count - a.squareSameValueList.Count);

        return squareMergeOrderByCountSameValueList;
    }

    private void GetCountSquareSameValueList(SquareData block, List<SquareData> squareSameValueList)
    {
        foreach (var squareData in squaresData)
        {
            // Debug.Log($"can merge {IsBlockCanMerge(squareData, block)}");
            if (IsBlockCanMerge(squareData, block))
            {
                squareSameValueList.Add(squareData);
            }
        }
    }

    private bool IsBlockCanMerge(SquareData squareData, SquareData block)
    {
        var isHasValue = squareData.value > 0;
        var isSameValue = Mathf.Approximately(block.value, squareData.value);

        var squareColumn = squareData.cell.Column;
        var squareRow = squareData.cell.Row;
        var blockColumn = block.cell.Column;
        var blockRow = block.cell.Row;

        var isSquareRight = squareColumn == blockColumn + 1 && squareRow == blockRow;
        var isSquareLeft = squareColumn == blockColumn - 1 && squareRow == blockRow;
        var isSquareDown = squareRow == blockRow + 1 && squareColumn == blockColumn;
        var isSquareUp = squareRow == blockRow - 1 && squareColumn == blockColumn;

        return isHasValue && isSameValue && (isSquareRight || isSquareLeft || isSquareDown || isSquareUp);
    }

    private void MergeSingleBlock(SquareData squareDataTarget, SquareData squareDataSource)
    {
        if (!Mathf.Approximately(squareDataSource.value, squareDataTarget.value))
        {
            return;
        }

        SquareData squareSource;
        SquareData squareTarget;
        var action = new StepAction();

        var newValue = squareDataTarget.value * 2;
        var isSameColumn = squareDataSource.cell.Row == squareDataTarget.cell.Row - 1;
        var isSameRowRight = squareDataTarget.cell.Column > squareDataSource.cell.Column &&
                             squareDataSource.cell.Column >= _processingSquare.cell.Column;
        var isSameRowLeft = squareDataTarget.cell.Column < squareDataSource.cell.Column &&
                            squareDataSource.cell.Column <= _processingSquare.cell.Column;

        if (isSameColumn || isSameRowRight || isSameRowLeft)
        {
            squareSource = squareDataTarget;
            squareTarget = squareDataSource;
        }
        else
        {
            squareSource = squareDataSource;
            squareTarget = squareDataTarget;
        }

        action.multiSquareSources.Add(new SquareData(squareSource.cell, squareSource.id, squareSource.value));
        action.squareTarget = new SquareData(squareTarget.cell, squareTarget.id, squareTarget.value);
        action.newSquareValue = newValue;

        _actionsList.Add(action);
        squareTarget.value = newValue;
        squareSource.value = 0;

        if (squareSource == _processingSquare)
        {
            _processingSquare = squareTarget;
        }
    }

    private void MergeMultiBlock(SquareData squareTarget, List<SquareData> squareSourceList)
    {
        var isCompareAllValue = IsCompareAllValue(squareTarget, squareSourceList);

        if (!isCompareAllValue)
        {
            return;
        }

        var countBlockSameValue = squareSourceList.Count();

        var newValue = CreateActionMultiMerge(squareTarget, squareSourceList, countBlockSameValue);

        squareTarget.value = newValue;
    }

    private float CreateActionMultiMerge(SquareData squareTarget, List<SquareData> squareSourceList,
        int countBlockSameValue)
    {
        var action = new StepAction();

        var newValue = squareTarget.value * (int)Mathf.Pow(2, countBlockSameValue);

        action.squareTarget = new SquareData(squareTarget.cell, squareTarget.id, squareTarget.value);
        action.newSquareValue = newValue;

        foreach (var squareData in squareSourceList)
        {
            action.multiSquareSources.Add(new SquareData(squareData.cell, squareData.id, squareData.value));
            squareData.value = 0;
            if (squareData == _processingSquare)
            {
                _processingSquare = squareTarget;
            }
        }

        _actionsList.Add(action);
        return newValue;
    }

    private static bool IsCompareAllValue(SquareData squareTarget, List<SquareData> squareDataSourceList)
    {
        foreach (var squareDataSameValue in squareDataSourceList)
        {
            if (!Mathf.Approximately(squareDataSameValue.value, squareTarget.value))
            {
                return false;
            }
        }

        return true;
    }

    #endregion

    private void SortAllBlock()
    {
        _actionsList.Clear();
        var emptyBlocksUpRowList = GetEmptyBlocksUpRowList();

        if (emptyBlocksUpRowList.Count == 0)
        {
            return;
        }

        foreach (var emptyBlocksUpRow in emptyBlocksUpRowList)
        {
            SortBlockBySquaresDataHasValueDownRowByCell(emptyBlocksUpRow);
        }

        if (_actionsList.Count == 0)
        {
            return;
        }

        _actionsWrapList.Add(new BoardAction(new List<StepAction>(_actionsList), ActionType.SortAllBlock));
    }

    #region SortBlock

    private List<SquareData> GetEmptyBlocksUpRowList()
    {
        var emptyBlocksUpRowList = new List<SquareData>();
        foreach (var item in squaresData)
        {
            if (item.value == 0 && IsSquareUpRow(item))
            {
                emptyBlocksUpRowList.Add(item);
            }
        }

        return emptyBlocksUpRowList;
    }

    private bool IsSquareUpRow(SquareData item)
    {
        foreach (var squareDownRow in squaresData)
        {
            var isUpRow = squareDownRow.cell.Row == item.cell.Row + 1;
            var isSameColumn = squareDownRow.cell.Column == item.cell.Column;
            if (isUpRow && isSameColumn && squareDownRow.value > 0) return true;
        }

        return false;
    }

    private void SortBlockBySquaresDataHasValueDownRowByCell(SquareData squareEmptyUpRow)
    {
        foreach (SquareData squareData in squaresData)
        {
            var isSameColumn = squareData.cell.Column == squareEmptyUpRow.cell.Column;
            var isSquareEmptyUpRow = squareData.cell.Row > squareEmptyUpRow.cell.Row;
            var isHasValue = squareData.value > 0;

            if (isSameColumn && isHasValue && isSquareEmptyUpRow)
            {
                SortBlock(squareData.cell,
                    new Utils.Cell(squareData.cell.Column, squareData.cell.Row - 1));
            }
        }
    }

    private void SortBlock(Utils.Cell cellSource, Utils.Cell cellTarget)
    {
        var action = new StepAction();
        var squareTarget = GetSquareDataByCell(cellTarget);
        var squareSource = GetSquareDataByCell(cellSource);

        action.singleSquareSources = new SquareData(squareSource.cell, squareSource.id, squareSource.value);
        action.squareTarget = new SquareData(squareTarget.cell, squareTarget.id, squareTarget.value);
        action.newSquareValue = squareSource.value;

        _actionsList.Add(action);

        squareTarget.value = squareSource.value;
        squareSource.value = 0;
        squareTarget.id = squareSource.id;
        squareSource.id = GetSquareSourceID(squareSource);

        if (squareSource == _processingSquare)
        {
            _processingSquare = squareTarget;
        }
    }

    private SquareData GetSquareDataByCell(Utils.Cell cell)
    {
        foreach (var squareData in squaresData)
        {
            if (squareData.cell.Row == cell.Row && squareData.cell.Column == cell.Column)
            {
                return squareData;
            }
        }

        return null;
    }

    private int GetSquareSourceID(SquareData squareSource)
    {
        return squareSource.cell.Column + squareSource.cell.Row * boardCol + 1;
    }

    #endregion

    public void SetNextSquareValue(GamePlayScreen gamePlayScreen = null)
    {
        if (gamePlayScreen is not null)
        {
            _gamePlayScreen = gamePlayScreen;
        }

        _gamePlayScreen.SetNextSquare();
    }

    private void SetRandomSquareValue()
    {
        SetNewValueInSquareValueList();
        SetRandomValue();
    }

    #region NextSquareValue

    private void SetRandomValue()
    {
        var countValueList = _squareValueList.Count;
        // var probabilityList = _probabilityList.Take(countValueList).ToList();
        List<int> probabilityList = new();
        for (var i = 0; i < countValueList; i++)
        {
            probabilityList.Add(_probabilityList[i]);
        }

        var maxValue = probabilityList[^1];
        var randomNum = Random.Range(0, 1f) * maxValue;

        for (var i = 0; i < countValueList; i++)
        {
            if (probabilityList[i] > randomNum)
            {
                var value = _squareValueList[countValueList - 1 - i];
                nextSquareValue = value;
                if (_gamePlayScreen is not null)
                {
                    SetNextSquareValue();
                }

                return;
            }
        }
    }

    private void SetNewValueInSquareValueList()
    {
        var maxValueInBoard = squaresData.Max(square => square.value);
        var maxValueInSquareValueList = _squareValueList[^1];

        var isEntryAddNewValue = maxValueInBoard > 8 &&
                                 maxValueInBoard / (Utils.GetExponent(maxValueInBoard) + 3) > maxValueInSquareValueList;
        if (isEntryAddNewValue)
        {
            _squareValueList.Add(maxValueInSquareValueList * 2);
        }

        if (_squareValueList.Count <= MAX_COUNT_QUARE_VALUE_LIST - 1)
        {
            return;
        }

        var minValueInBoard = _squareValueList[0];
        _squareValueList.RemoveAt(0);

        ClearMinBlock(minValueInBoard);
    }

    private void ClearMinBlock(float minValueInBoard)
    {
        _actionsList.Clear();

        foreach (var squareData in squaresData)
        {
            if (Mathf.Approximately(squareData.value, minValueInBoard))
            {
                StepAction action = new()
                {
                    squareTarget = new SquareData(squareData.cell, squareData.id, squareData.value)
                };

                squareData.value = 0;
                _actionsList.Add(action);
            }
        }

        var item = new BoardAction(new List<StepAction>(_actionsList), ActionType.ClearMinBlock);
        _actionsWrapList.Add(item);

        ProcessingLoop();
    }

    #endregion

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        CheckSaveGame(pauseStatus);
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        CheckSaveGame(!hasFocus);
    }

    #region SaveGame

    private void CheckSaveGame(bool isPause)
    {
        if (isPause)
        {
            SaveGame();
        }

        if (!isPause)
        {
            _isSave = false;
        }
    }

    private void SaveGame()
    {
        if (_isSave)
        {
            return;
        }

        _isSave = true;

        var jsonHelperSquaresData = new Utils.JsonHelper<SquareData>(squaresData);
        var jsonHelperValueList = new Utils.JsonHelper<float>(_squareValueList);

        Prefs.SquaresData = JsonUtility.ToJson(jsonHelperSquaresData);
        Prefs.Score = score;
        Prefs.HighScore = highScore;
        Prefs.IdCount = idCount;
        Prefs.SquareValueList = JsonUtility.ToJson(jsonHelperValueList);
        Prefs.NextSquareValue = nextSquareValue;
    }

    #endregion

    private void LoadDataFromPrefs()
    {
        RenderBoard();
        if (!string.IsNullOrEmpty(Prefs.SquaresData))
        {
            squaresData = JsonUtility.FromJson<Utils.JsonHelper<SquareData>>(Prefs.SquaresData).data;
        }

        score = Prefs.Score;
        idCount = Prefs.IdCount == 0 ? 30 : Prefs.IdCount;
        _uiManager.idCount = idCount;

        LoadHighScore();
        LoadSquareValueList();

        nextSquareValue = Prefs.NextSquareValue;
    }

    #region LoadGame

    private void LoadSquareValueList()
    {
        if (string.IsNullOrWhiteSpace(Prefs.SquareValueList) || string.IsNullOrEmpty(Prefs.SquareValueList))
        {
            return;
        }

        var numsList = JsonUtility.FromJson<Utils.JsonHelper<float>>(Prefs.SquareValueList).data;
        var valueListPrefs = new List<float>();
        foreach (var value in numsList)
        {
            if (value > 0)
            {
                valueListPrefs.Add(value);
            }
        }


        if (valueListPrefs.Count > 0)
        {
            _squareValueList = valueListPrefs;
        }
    }

    private void LoadHighScore()
    {
        highScore = Prefs.HighScore;
    }

    #endregion
}