using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Profiling;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : Singleton<BoardManager>
{
    #region NewCode

    [SerializeField] private GameObject lineColumn;
    [SerializeField] private Transform lineParentTransform;
    [SerializeField] private Square nextSquare;

    public List<SquareData> squaresData = new();
    public int boardRow = 5;
    public int boardCol = 6;
    public bool isTouchLine;
    public int columnSelect;
    public bool isProcessing;
    public float score;
    public float highScore;
    public int idCount;
    public float nextSquareValue;
    public bool isClearData;
    public bool isGameOver;

    private bool _isSave;
    private bool _isMaxItemColumn;
    private UIManager _uiManager;
    private SquareData _processingSquare;
    private List<GameObject> _lineColumnList = new();
    private List<StepAction> _actionsList = new();
    private List<BoardAction> _actionsWrapList = new();
    private List<long> _squareValueList = new() { 2, 4 };
    // private List<int> _squareValueList = new() { 2, 4, 8, 16, 32, 64, 128 };
    private readonly int[] _probabilityList = { 1, 4, 10, 18, 28, 30, 44, 60, 78 };
    private static readonly ProfilerMarker ProcessingDataMaker = new("MyMaker.ProcessingData");
    private static readonly ProfilerMarker RenderUIMaker = new("MyMaker.RenderUI");
    
    private const int MAX_COUNT_QUARE_VALUE_LIST = 9;

    private void Start()
    {
        // Application.targetFrameRate = 60;
        _uiManager = UIManager.Instance;
        RenderLineColumn();
        if (isClearData || nextSquareValue < 1)
        {
            RestartGame();
        }
        else
        {
            LoadDataFromPrefs();
            nextSquare.SetValue(nextSquareValue);
        }

        _uiManager.StartUI(squaresData);
    }

    public void RestartGame()
    {
        LoadHighScore();
        ResetBoard();
        SetRandomSquareValue();
    }

    public IEnumerator ShootBlock()
    {
        isProcessing = true;
        _actionsWrapList.Clear();
        yield return new WaitForNextFrameUnit();

        ProcessingDataMaker.Begin();
        ProcessingData(columnSelect);

        ProcessingDataMaker.End();

        yield return new WaitForNextFrameUnit();
        if (!_isMaxItemColumn)
        {
            SetRandomSquareValue();
        }

        yield return new WaitForNextFrameUnit();

        RenderUIMaker.Begin();
        _uiManager.RenderUI(_actionsWrapList);
        RenderUIMaker.End();

        yield return new WaitForNextFrameUnit();

        CheckGameOver();

        // foreach (var actionListWrap in _actionsWrapList)
        // {
        //     Debug.Log("----actionListWrap: " + JsonUtility.ToJson(actionListWrap));
        // }
    }

    private void ProcessingData(int column)
    {
        Shoot(column);

        if (_isMaxItemColumn)
        {
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
        } while (countActionsList < _actionsWrapList.Count && countActionsList < 30);
    }

    private void Shoot(int column)
    {
        _actionsList.Clear();
        _isMaxItemColumn = false;
        var action = new StepAction();
        var squareTarget = GetEmptySquareDataTargetByColumn(column);
        if (squareTarget == null)
        {
            _isMaxItemColumn = true;
            return;
        }

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

    private SquareData GetEmptySquareDataTargetByColumn(int column)
    {
        return squaresData.Find(block => block.value == 0 && block.cell.Column == column);
    }

    private void ResetBoard()
    {
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

    private void MergeAllBlock()
    {
        _actionsList.Clear();

        var squareMergeOrderByCountSameValueList = squaresData
            .Where(block => block.value > 0)
            .Select(block => new
            {
                block,
                SquareSameValueList = squaresData.Where(squareData => IsBlockCanMerge(squareData, block)).ToArray()
            })
            .Where(data => data.SquareSameValueList.Any())
            .OrderByDescending(data => data.SquareSameValueList.Count());

        foreach (var data in squareMergeOrderByCountSameValueList)
        {
            if (data.SquareSameValueList.Count() == 1)
            {
                MergeSingleBlock(data.block, data.SquareSameValueList.First());
            }
            else
            {
                MergeMultiBlock(data.block, data.SquareSameValueList);
            }
        }

        if (!_actionsList.Any())
        {
            return;
        }

        _actionsWrapList.Add(new BoardAction(new List<StepAction>(_actionsList), ActionType.MergeAllBlock));
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
        // squareSource.id = GetSquareSourceID(squareSource);
        if (squareSource == _processingSquare)
        {
            _processingSquare = squareTarget;
        }
    }

    private void MergeMultiBlock(SquareData squareTarget, IEnumerable<SquareData> squareSourceList)
    {
        var squareDataSourceList = squareSourceList as SquareData[] ?? squareSourceList.ToArray();
        var isCompareAllValue =
            squareDataSourceList.All(squareDataSameValue =>
                Mathf.Approximately(squareDataSameValue.value, squareTarget.value));
        if (!isCompareAllValue)
        {
            return;
        }

        var countBlockSameValue = squareDataSourceList.Count();

        var action = new StepAction();

        var newValue = squareTarget.value * (int)Mathf.Pow(2, countBlockSameValue);

        action.squareTarget = new SquareData(squareTarget.cell, squareTarget.id, squareTarget.value);
        action.newSquareValue = newValue;

        foreach (var squareData in squareDataSourceList)
        {
            action.multiSquareSources.Add(new SquareData(squareData.cell, squareData.id, squareData.value));
            squareData.value = 0;
            if (squareData == _processingSquare)
            {
                _processingSquare = squareTarget;
            }
        }

        _actionsList.Add(action);

        squareTarget.value = newValue;
    }

    private SquareData GetSquareDataByCell(Utils.Cell cell)
    {
        return squaresData.Find(item => item.cell.Row == cell.Row && item.cell.Column == cell.Column);
    }

    private void SortAllBlock()
    {
        _actionsList.Clear();
        var emptyBlocksUpRowList = squaresData
            .FindAll(item => item.value == 0 &&
                             squaresData.Any(squareDownRow =>
                                 squareDownRow.cell.Row == item.cell.Row + 1 &&
                                 squareDownRow.cell.Column == item.cell.Column &&
                                 squareDownRow.value > 0));

        if (!emptyBlocksUpRowList.Any())
        {
            return;
        }

        foreach (var emptyBlocksUpRow in emptyBlocksUpRowList)
        {
            var squareHasValueDownRowList = GetSquaresDataHasValueDownRowByCell(emptyBlocksUpRow);

            foreach (var squareHasValueDownRow in squareHasValueDownRowList)
            {
                SortBlock(squareHasValueDownRow.cell,
                    new Utils.Cell(squareHasValueDownRow.cell.Column, squareHasValueDownRow.cell.Row - 1));
            }
        }

        if (!_actionsList.Any())
        {
            return;
        }

        _actionsWrapList.Add(new BoardAction(new List<StepAction>(_actionsList), ActionType.SortAllBlock));
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

    private int GetSquareSourceID(SquareData squareSource)
    {
        return squareSource.cell.Column + squareSource.cell.Row * boardCol + 1;
    }

    private IEnumerable<SquareData> GetSquaresDataHasValueDownRowByCell(SquareData squareEmptyUpRow)
    {
        return squaresData.Where(squareData => squareData.cell.Column == squareEmptyUpRow.cell.Column &&
                                               squareData.value > 0 &&
                                               squareData.cell.Row > squareEmptyUpRow.cell.Row)
                .OrderBy(squareData => squareData.cell.Row)
            ;
    }

    private void SetRandomSquareValue()
    {
        SetNewNextValue();
        SetRandomValue();
    }

    private void SetRandomValue()
    {
        var countValueList = _squareValueList.Count;
        var probabilityList = _probabilityList.Take(countValueList).ToList();
        var maxValue = probabilityList[^1];
        var randomNum = Random.Range(0, 1f) * maxValue;

        for (var i = 0; i < countValueList; i++)
        {
            if (probabilityList[i] > randomNum)
            {
                var value = _squareValueList[countValueList - 1 - i];
                nextSquareValue = value;
                nextSquare.SetValue(value);
                return;
            }
        }
    }

    private void SetNewNextValue()
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

    private void ClearMinBlock(long minValueInBoard)
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

    private void CheckGameOver()
    {
        isGameOver = squaresData.All(squareData => squareData.value > 0);
    }

    #endregion

    #region SaveAndLoadGame

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

        var jsonHelper = new Utils.JsonHelper<SquareData>(squaresData);

        Prefs.SquaresData = JsonUtility.ToJson(jsonHelper);
        Prefs.Score = score;
        Prefs.HighScore = highScore;
        Prefs.IdCount = idCount;
        Prefs.NextSquareValue = nextSquareValue;
        Prefs.SquareValueList = JsonUtility.ToJson(_squareValueList);
    }

    private void LoadDataFromPrefs()
    {
        squaresData = JsonUtility.FromJson<Utils.JsonHelper<SquareData>>(Prefs.SquaresData)?.datas;
        score = Prefs.Score;
        LoadHighScore();
        idCount = Prefs.IdCount;
        nextSquareValue = Prefs.NextSquareValue;

        _squareValueList = JsonUtility.FromJson<Utils.JsonHelper<long>>(Prefs.SquaresData)?.datas;

        _uiManager.idCount = idCount;
    }

    private void LoadHighScore()
    {
        highScore = Prefs.HighScore;
    }

    #endregion
}