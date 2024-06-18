using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Profiling;
using Unity.VisualScripting;
using UnityEngine;

public class BoardManager : Singleton<BoardManager>
{
    #region NewCode

    [SerializeField] private List<SquareData> squareDatas = new();
    [SerializeField] private GameObject lineColumn;
    [SerializeField] private Transform lineParentTransform;
    [SerializeField] private Square nextSquare;

    public int boardRow = 5;
    public int boardCol = 6;
    public bool isTouchLine;
    public int columnSelect;
    public bool isProcessing;
    public int score;

    private UIManager _uiManager;
    private SquareData _processingSquare;
    private List<GameObject> _lineColumnList = new();
    private List<StepAction> _actionsList = new();
    private List<BoardAction> _actionsWrapList = new();

    private List<int> _squareValueList = new() { 2, 4, 8 };

    // private List<int> _squareValueList = new() { 2, 4, 8, 16, 32, 64, 128 };
    private int _newSquareValue;
    private int _randomNum;
    private int _idCount;
    private const int MAX_COUNT_QUARE_VALUE_LIST = 6;

    private static readonly ProfilerMarker ProcessingDataMaker = new("MyMaker.ProcessingData");
    private static readonly ProfilerMarker RenderUIMaker = new("MyMaker.RenderUI");


    public List<SquareData> SquareDatas
    {
        get => squareDatas;
        set => squareDatas = value;
    }

    public void SetActiveTouch(bool isActive)
    {
        foreach (var line in _lineColumnList)
        {
            line.SetActive(isActive);
        }
    }

    private void Start()
    {
        // Application.targetFrameRate = 60;
        _uiManager = UIManager.Instance;
        ResetBoard();
        RenderLineColumn();
        SetRandomSquareValue();

        _uiManager.StartUI(squareDatas);
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
        SetRandomSquareValue();

        yield return new WaitForNextFrameUnit();

        RenderUIMaker.Begin();
        _uiManager.RenderUI(_actionsWrapList);
        RenderUIMaker.End();

        yield return new WaitForNextFrameUnit();

        // foreach (var actionListWrap in _actionsWrapList)
        // {
        //     Debug.Log("----actionListWrap: " + JsonUtility.ToJson(actionListWrap));
        // }
    }

    private void ProcessingData(int column)
    {
        Shoot(column);
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
        var action = new StepAction();
        var squareTarget = GetEmptySquareDataTargetByColumn(column);
        if (squareTarget == null)
        {
            return;
        }

        _idCount++;

        var squareSource = new SquareData(
            new Utils.Cell(column, boardRow),
            _idCount,
            _newSquareValue
        );

        action.squareTarget = new SquareData(squareTarget.cell, squareTarget.id, squareTarget.value);
        action.squareSources.Add(new SquareData(squareSource.cell, squareSource.id, squareSource.value));
        action.newSquareValue = _newSquareValue;

        _actionsList.Add(action);
        var item = new BoardAction(new List<StepAction>(_actionsList), ActionType.Shoot);
        _actionsWrapList.Add(item);

        squareTarget.id = squareSource.id;
        squareSource.id = 0;
        squareTarget.value = _newSquareValue;
        _processingSquare = squareTarget;
    }

    private SquareData GetEmptySquareDataTargetByColumn(int column)
    {
        return squareDatas.Find(block => block.value == 0 && block.cell.Column == column);
    }

    private void ResetBoard()
    {
        for (var y = boardRow; y > 0; y--)
        {
            for (var x = 0; x < boardCol; x++)
            {
                _idCount++;
                squareDatas.Add(new SquareData(
                    new Utils.Cell(x, boardRow - y),
                    _idCount,
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

        // foreach (var squareData in squareDatas)
        // {
        //     if (squareData.value == 0)
        //     {
        //         continue;
        //     }
        //
        //     var squareSameValueList = new List<SquareData>();
        //     foreach (var square in squareDatas)
        //     {
        //         if (IsBlockCanMerge(square, squareData))
        //         {
        //             squareSameValueList.Add(square);
        //         }
        //     }
        //     
        //
        // }

        // for(int j = 0; j < squareDatas.Count -1;j++)
        // {
        //     for(int i = 0;i < squareDatas.Count-1;i++ )
        //     {
        //         var squareSameValueList = GetSquareSamevalue(i);
        //         if (squareSameValueList.Count == 0)
        //         {
        //             continue;
        //         }
        //         
        //         // int swap = 0;
        //         // if(squareDatas[i] > squareDatas[ i+1])
        //         // {
        //         //     swap  = squareDatas[i+1];
        //         //     squareDatas[i+1] = squareDatas[i];
        //         //     squareDatas[i] = swap;
        //         // }
        //     }
        // }

        var squareMergeOrderByCountSameValueList = squareDatas
            .Where(block => block.value > 0)
            .Select(block => new
            {
                block,
                SquareSameValueList = squareDatas.Where(squareData => IsBlockCanMerge(squareData, block)).ToArray()
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

    private List<SquareData> GetSquareSamevalue(int i)
    {
        var squareSameValueList = new List<SquareData>();
        foreach (var square in squareDatas)
        {
            if (IsBlockCanMerge(square, squareDatas[i]))
            {
                squareSameValueList.Add(square);
            }
        }

        return squareSameValueList;
    }

    private bool IsBlockCanMerge(SquareData squareData, SquareData block)
    {
        var isHasValue = squareData.value > 0;
        var isSameValue = block.value == squareData.value;

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
        if (squareDataSource.value != squareDataTarget.value)
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

        action.squareSources.Add(new SquareData(squareSource.cell, squareSource.id, squareSource.value));
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
            squareDataSourceList.All(squareDataSameValue => squareDataSameValue.value == squareTarget.value);
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
            action.squareSources.Add(new SquareData(squareData.cell, squareData.id, squareData.value));
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
        return squareDatas.Find(item => item.cell.Row == cell.Row && item.cell.Column == cell.Column);
    }

    private void SortAllBlock()
    {
        _actionsList.Clear();
        var emptyBlocksUpRowList = squareDatas
            .FindAll(item => item.value == 0 &&
                             squareDatas.Any(squareDownRow =>
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

        action.squareSources.Add(new SquareData(squareSource.cell, squareSource.id, squareSource.value));
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
        return squareDatas.Where(squareData => squareData.cell.Column == squareEmptyUpRow.cell.Column &&
                                               squareData.value > 0 &&
                                               squareData.cell.Row > squareEmptyUpRow.cell.Row)
                .OrderBy(squareData => squareData.cell.Row)
            ;
    }

    private void SetRandomSquareValue()
    {
        SetNewSquareValue();

        SetRandomValue();
        // _randomNum = Random.Range(0, _squareValueList.Count);
        // _newSquareValue = _squareValueList[_randomNum];
        // nextSquare.SetValue(_newSquareValue);
    }

    private readonly int[] _probabilityList = { 1, 3, 6, 10, 15, 21 };

    private void SetRandomValue()
    {
        var countValueList = _squareValueList.Count;
        var probabilityList = _probabilityList.Take(countValueList).ToList();
        var maxValue = probabilityList[^1];
        var randomNum = Random.Range(0, 1f) * maxValue;

        for (var i = 0; i < countValueList; i++)
        {
            // Debug.Log($"countValueList - 1 - i  {countValueList - 1 - i} i {i}");
            if (probabilityList[i] > randomNum)
            {
                var value = _squareValueList[countValueList - 1 - i];
                _newSquareValue = value;
                _randomNum = value;
                nextSquare.SetValue(value);
                return;
            }
        }
    }

    private void SetNewSquareValue()
    {
        var maxValueInBoard = squareDatas.Max(square => square.value);
        var maxValueInSquareValueList = _squareValueList[^1];
        // Debug.Log($"maxValueInBoard {maxValueInBoard} maxValueInSquareValueList {maxValueInSquareValueList}");

        if (maxValueInBoard / 4 > maxValueInSquareValueList)
        {
            _squareValueList.Add(maxValueInSquareValueList * 2);
        }

        if (_squareValueList.Count > MAX_COUNT_QUARE_VALUE_LIST - 3) //todo
        {
            Debug.Log("DEL MIN VALUE");
            var minValueInBoard = _squareValueList[0];
            _squareValueList.RemoveAt(0);

            ClearMinBlock(minValueInBoard);
        }

        Debug.Log("_squareValueList  " + string.Join(" - ", _squareValueList));
    }

    private void ClearMinBlock(int minValueInBoard)
    {
        _actionsList.Clear();

        StepAction action = new();
        foreach (var squareData in squareDatas)
        {
            // Debug.Log($"squareData.value {squareData.value} minValueInBoard {minValueInBoard}");
            if (squareData.value == minValueInBoard)
            {
                squareData.value = 0;
                action.squareSources.Add(squareData);
            }
        }

        _actionsList.Add(action);
        foreach (var stepAction in _actionsList)
        {
            Debug.Log($"_actionsList {JsonUtility.ToJson(stepAction)}");
        }
        var item = new BoardAction(new List<StepAction>(_actionsList), ActionType.ClearMinBlock);
        _actionsWrapList.Add(item);
        
        ProcessingLoop();
    }

    #endregion
}