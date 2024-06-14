using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardManager : Singleton<BoardManager>
{
    #region NewCode

    [SerializeField] private List<SquareData> squareDatas = new();
    [SerializeField] private List<Square> squares = new();
    [SerializeField] private GameObject lineColumn;
    [SerializeField] private Transform lineParentTransform;


    public int boardRow = 5;
    public int boardCol = 6;
    public bool isTouchLine;
    public int columnSelect;

    private List<Square> _listSquare = new();
    private List<GameObject> _listLineColumn = new();
    private SquareData _processingSquare;
    private List<MergerAction> _actionsList = new();
    private readonly List<int> _listSquareValue = new() { 2 };
    // private readonly List<int> _listSquareValue = new() { 2, 4, 8, 16, 32, 64, 128 };

    private int _randomNum;
    private int _newSquareValue;

    private void Start()
    {
        ResetBoard();
        RenderLineColumn();
        SetRandomSquareValue();
        TestData();
    }

    private void TestData()
    {
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

    public void Shoot(int column)
    {
        var action = new MergerAction();
        var squareTarget = GetEmptySquareDataTargetByColumn(column);
        var squareSource = new SquareData(
            new Utils.Cell(column, boardRow + 1),
            squareTarget.index,
            _newSquareValue
        );

        action.squareTarget = squareTarget;
        action.squareSources.Add(squareSource);
        action.newSquareValue = _newSquareValue;
        action.type = ActionType.Shoot;

        _actionsList.Add(action);

        squareTarget.value = _newSquareValue;
        _processingSquare = squareTarget;

        ProcessingData();

        // Sequence mySequence = DOTween.Sequence();
        // mySequence.Append(transform.DOMoveX(45, 1))
        // mySequence.Append(transform.DOMoveX(45, 1))
    }

    private void ProcessingData()
    {
        int countActionsList;

        do
        {
            countActionsList = _actionsList.Count;

            MergeAllBlock();
            if (countActionsList == _actionsList.Count)
            {
                break;
            }

            SortAllBlock();
        } while (countActionsList < _actionsList.Count);

        _actionsList.ForEach(e => { Debug.Log(JsonUtility.ToJson(e)); });
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
            line.GetComponent<LineColumn>().Column = i;
            _listLineColumn.Add(line);
        }
    }

    private void MergeAllBlock()
    {
        var squareMergeOrderByCountSameValueList = squareDatas
            .Where(block => block.value > 0)
            .Select(block => new
            {
                block,
                countSquareSameValue = squareDatas.Count(squareData => IsBlockCanMerge(squareData, block))
            })
            .Where(data => data.countSquareSameValue > 0)
            .OrderByDescending(data => data.countSquareSameValue);

        foreach (var data in squareMergeOrderByCountSameValueList)
        {
            if (data.countSquareSameValue == 1)
            {
                MergeBlock(data.block);
            }
            else
            {
                MergeMultiBlock(data.block);
            }
        }
    }

    private bool IsBlockCanMerge(SquareData squareData, SquareData block)
    {
        var isHasValue = squareData.value > 0;
        var isSameValue = block.value == squareData.value;
        var isSquareRight = squareData.index == block.index + 1;
        var isSquareLeft = squareData.index == block.index - 1;
        var isSquareDown = squareData.index == block.index + boardCol;
        var isSquareUp = squareData.index == block.index - boardCol;
        return isHasValue && isSameValue && (isSquareRight || isSquareLeft || isSquareDown || isSquareUp);
    }

    private void MergeBlock(SquareData block)
    {
        SquareData squareSource;
        SquareData squareTarget;
        var action = new MergerAction();

        SquareData squareDataSameValue = squareDatas.Find(squareData => IsBlockCanMerge(squareData, block));

        if (squareDataSameValue is null)
        {
            return;
        }

        var newValue = block.value * 2;
        var isSameColumn = squareDataSameValue.cell.Row == block.cell.Row - 1;
        var isSameRowRight = block.cell.Column > squareDataSameValue.cell.Column &&
                             squareDataSameValue.cell.Column >= _processingSquare.cell.Column;
        var isSameRowLeft = block.cell.Column < squareDataSameValue.cell.Column &&
                            squareDataSameValue.cell.Column <= _processingSquare.cell.Column;

        if (isSameColumn || isSameRowRight || isSameRowLeft)
        {
            squareSource = block;
            squareTarget = squareDataSameValue;
        }
        else
        {
            squareSource = squareDataSameValue;
            squareTarget = block;
        }

        action.squareSources.Add(squareSource);
        action.squareTarget = new SquareData(squareTarget.cell, squareTarget.index, squareTarget.value);
        action.newSquareValue = newValue;
        action.type = ActionType.MergeBlock;

        _actionsList.Add(action);
        squareTarget.value = newValue;
        squareSource.value = 0;
        if (squareSource == _processingSquare)
        {
            _processingSquare = squareTarget;
        }
    }

    private void MergeMultiBlock(SquareData cellCheck)
    {
        var action = new MergerAction();
        var countBlockSameValue = 0;

        var squareLeft = GetSquareDataByCell(new Utils.Cell(cellCheck.cell.Column - 1, cellCheck.cell.Row));
        var squareUp = GetSquareDataByCell(new Utils.Cell(cellCheck.cell.Column, cellCheck.cell.Row - 1));
        var squareRight = GetSquareDataByCell(new Utils.Cell(cellCheck.cell.Column + 1, cellCheck.cell.Row));
        var squareDown = GetSquareDataByCell(new Utils.Cell(cellCheck.cell.Column, cellCheck.cell.Row + 1));

        countBlockSameValue = CountBlockSameValue(squareLeft, cellCheck, countBlockSameValue, action);
        countBlockSameValue = CountBlockSameValue(squareUp, cellCheck, countBlockSameValue, action);
        countBlockSameValue = CountBlockSameValue(squareRight, cellCheck, countBlockSameValue, action);
        countBlockSameValue = CountBlockSameValue(squareDown, cellCheck, countBlockSameValue, action);

        if (countBlockSameValue == 0)
        {
            return;
        }

        var newValue = cellCheck.value * (int)Mathf.Pow(2, countBlockSameValue);

        action.squareTarget = new SquareData(cellCheck.cell, cellCheck.index, cellCheck.value);
        action.newSquareValue = newValue;
        action.type = ActionType.MergeMultiBlock;

        _actionsList.Add(action);

        cellCheck.value = newValue;
    }

    private SquareData GetSquareDataByCell(Utils.Cell cell)
    {
        return squareDatas.Find(item => item.cell.Row == cell.Row && item.cell.Column == cell.Column);
    }

    private int CountBlockSameValue(
        SquareData squareData,
        SquareData cellCheck,
        int countBlockSameValue,
        MergerAction action)
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
    }

    private void SortBlock(Utils.Cell cellCheck, Utils.Cell cellUp)
    {
        var action = new MergerAction();
        var squareUp = GetSquareDataByCell(cellUp);
        var squareCheck = GetSquareDataByCell(cellCheck);

        action.squareSources.Add(new SquareData(squareCheck.cell, squareCheck.index, squareCheck.value));
        action.squareTarget = new SquareData(squareUp.cell, squareUp.index, squareUp.value);
        action.newSquareValue = squareCheck.value;
        action.type = ActionType.SortBlock;

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

    public int GetIndexByCell(Utils.Cell cell)
    {
        return cell.Column + cell.Row * boardCol;
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
        _randomNum = Random.Range(0, _listSquareValue.Count);
        _newSquareValue = _listSquareValue[_randomNum];
    }

    #endregion
}