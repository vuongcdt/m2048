using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
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

    private UIManager _uiManager;
    private int _randomNum;
    private int _newSquareValue;
    private SquareData _processingSquare;
    private List<GameObject> _lineColumnList = new();
    private List<StepAction> _actionsList = new();
    private List<BoardAction> _actionsWrapList = new();
    private List<int> _squareValueList = new() { 2, 4 };
    // private readonly List<int> _listSquareValue = new() { 2, 4, 8, 16, 32, 64, 128 };

    private void Start()
    {
        _uiManager = UIManager.Instance;
        ResetBoard();
        RenderLineColumn();
        SetRandomSquareValue();

        // ShootBlock(1);
        // TestData();
    }

    private void TestData()
    {
        // squareDatas[0].value = 2;
        // squareDatas[6].value = 4;

        squareDatas[0].value = 16;
        squareDatas[1].value = 2;
        squareDatas[2].value = 16;

        squareDatas[6].value = 2;
        // squareDatas[7].value = 2;
        squareDatas[8].value = 2;

        squareDatas[12].value = 64;
        // var cellCheck = GetSquareDataByCell(new Utils.Cell(1, 1));


        // Shoot(1);

        ////

        // squareDatas[2].value = 2;
        // // squareDatas[8].value = 2;
        // Shoot(3);

        // var cellCheck = GetSquareDataByCell(new Utils.Cell(2, 1));
        // _processingSquare = cellCheck;

        // squareDatas[0].value = 2;
        // squareDatas[1].value = 8;


        // squareDatas[2].value = 2;
        //
        // squareDatas[6].value = 8;
        // squareDatas[7].value = 2;
        // // squareDatas[8].value = 2;
        //
        // squareDatas[12].value = 4;
        // squareDatas[13].value = 8;
        //
        // squareDatas[19].value = 4;
        // // var cellCheck = GetSquareDataByCell(new Utils.Cell(2, 1));
        // ////
        // Shoot(2);
    }

    public void ShootBlock()
    {
        _actionsWrapList.Clear();
        ProcessingData(columnSelect);

        Debug.Log("------");
        foreach (var actionListWrap in _actionsWrapList)
        {
            Debug.Log("actionListWrap: " + JsonUtility.ToJson(actionListWrap));
        }

        Debug.Log("......");

        _uiManager.RenderUI(_actionsWrapList);

        SetRandomSquareValue();
    }

    private void ProcessingData(int column)
    {
        int countActionsList;
        Shoot(column);
        do
        {
            countActionsList = _actionsWrapList.Count;

            MergeAllBlock();
            if (countActionsList == _actionsWrapList.Count)
            {
                break;
            }

            SortAllBlock();
        } while (countActionsList < _actionsWrapList.Count);
    }

    private void Shoot(int column)
    {
        _actionsList.Clear();
        var action = new StepAction();
        var squareTarget = GetEmptySquareDataTargetByColumn(column);
        var squareSource = new SquareData(
            new Utils.Cell(column, boardRow),
            boardRow * boardCol + column,
            _newSquareValue
        );

        action.squareTarget = new SquareData(squareTarget.cell,squareTarget.index,squareTarget.value);
        action.squareSources.Add(new SquareData(squareSource.cell,squareSource.index,squareSource.value));
        action.newSquareValue = _newSquareValue;

        _actionsList.Add(action);
        var item = new BoardAction(new List<StepAction>(_actionsList), ActionType.Shoot);
        _actionsWrapList.Add(item);

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
                squareDatas.Add(new SquareData(
                    new Utils.Cell(x, boardRow - y),
                    x + (boardRow - y) * boardCol,
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
                MergeBlock(data.block, data.SquareSameValueList.First());
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
        var isSameValue = block.value == squareData.value;
        var isSquareRight = squareData.cell.Column == block.cell.Column + 1 && squareData.cell.Row == block.cell.Row;
        var isSquareLeft = squareData.cell.Column == block.cell.Column - 1 && squareData.cell.Row == block.cell.Row;
        var isSquareDown = squareData.cell.Row == block.cell.Row + 1 && squareData.cell.Column == block.cell.Column;
        var isSquareUp = squareData.cell.Row == block.cell.Row - 1 && squareData.cell.Column == block.cell.Column;
        return isHasValue && isSameValue && (isSquareRight || isSquareLeft || isSquareDown || isSquareUp);
    }

    private void MergeBlock(SquareData squareDataTarget, SquareData squareDataSource)
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

        action.squareSources.Add(new SquareData(squareSource.cell, squareSource.index, squareSource.value));
        action.squareTarget = new SquareData(squareTarget.cell, squareTarget.index, squareTarget.value);
        action.newSquareValue = newValue;

        _actionsList.Add(action);
        squareTarget.value = newValue;
        squareSource.value = 0;
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

        //
        // var countBlockSameValue = 0;
        //
        // var squareLeft = GetSquareDataByCell(new Utils.Cell(squareTarget.cell.Column - 1, squareTarget.cell.Row));
        // var squareUp = GetSquareDataByCell(new Utils.Cell(squareTarget.cell.Column, squareTarget.cell.Row - 1));
        // var squareRight = GetSquareDataByCell(new Utils.Cell(squareTarget.cell.Column + 1, squareTarget.cell.Row));
        // var squareDown = GetSquareDataByCell(new Utils.Cell(squareTarget.cell.Column, squareTarget.cell.Row + 1));
        //
        // countBlockSameValue = CountBlockSameValue(squareLeft, squareTarget, countBlockSameValue, action);
        // countBlockSameValue = CountBlockSameValue(squareUp, squareTarget, countBlockSameValue, action);
        // countBlockSameValue = CountBlockSameValue(squareRight, squareTarget, countBlockSameValue, action);
        // countBlockSameValue = CountBlockSameValue(squareDown, squareTarget, countBlockSameValue, action);
        //
        // if (countBlockSameValue == 0)
        // {
        //     return;
        // }


        var newValue = squareTarget.value * (int)Mathf.Pow(2, countBlockSameValue);

        action.squareTarget = new SquareData(squareTarget.cell, squareTarget.index, squareTarget.value);
        action.newSquareValue = newValue;

        foreach (var squareData in squareDataSourceList)
        {
            action.squareSources.Add(new SquareData(squareData.cell, squareData.index, squareData.value));
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

    private int CountBlockSameValue(SquareData squareData, SquareData cellCheck, int countBlockSameValue,
        StepAction action)
    {
        if (squareData != null && squareData.value == cellCheck.value && squareData.value > 0)
        {
            action.squareSources.Add(new SquareData(squareData.cell, squareData.index, squareData.value));
            squareData.value = 0;
            countBlockSameValue++;
            if (squareData == _processingSquare)
            {
                _processingSquare = cellCheck;
            }
        }

        return countBlockSameValue;
    }

    private void SortAllBlock()
    {
        _actionsList.Clear();
        var emptyBlocksUpRowList = squareDatas
            .FindAll(item => item.value == 0 &&
                             squareDatas.Any(squareDownRow =>
                                 squareDownRow.index == item.index + boardCol &&
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

    private void SortBlock(Utils.Cell cellCheck, Utils.Cell cellUp)
    {
        var action = new StepAction();
        var squareUp = GetSquareDataByCell(cellUp);
        var squareCheck = GetSquareDataByCell(cellCheck);

        action.squareSources.Add(new SquareData(squareCheck.cell, squareCheck.index, squareCheck.value));
        action.squareTarget = new SquareData(squareUp.cell, squareUp.index, squareUp.value);
        action.newSquareValue = squareCheck.value;

        _actionsList.Add(action);

        squareUp.value = squareCheck.value;
        squareCheck.value = 0;

        if (squareCheck == _processingSquare)
        {
            _processingSquare = squareUp;
        }
    }

    private IEnumerable<SquareData> GetSquaresDataHasValueDownRowByCell(SquareData squareEmptyUpRow)
    {
        return squareDatas.Where(squareData => squareData.cell.Column == squareEmptyUpRow.cell.Column &&
                                               squareData.value > 0 &&
                                               squareData.cell.Row > squareEmptyUpRow.cell.Row)
                .OrderBy(squareData => squareData.index)
            ;
    }

    private void PrinterSquaresData()
    {
        print("__________");
        squareDatas
            .FindAll(e => e.value > 0)
            .ForEach(e => { Debug.Log(JsonUtility.ToJson(e)); });
    }

    private void SetRandomSquareValue()
    {
        _randomNum = Random.Range(0, _squareValueList.Count);
        _newSquareValue = _squareValueList[_randomNum];
        nextSquare.SetValue(_newSquareValue);
    }

    #endregion
}