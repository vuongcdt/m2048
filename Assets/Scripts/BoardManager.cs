using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardManager : Singleton<BoardManager>
{
    #region NewCode

    [SerializeField] private List<SquareData> squareDatas = new();
    [SerializeField] private List<Square> squares = new();

    private int _boardRow = 5;
    private int _boardCol = 6;
    private SquareData _processingSquare;

    private int _count;

    private List<MergerAction> _actionsList = new();

    private void Start()
    {
        ResetBoard();

        // squareDatas[1].value = 2;
        // squareDatas[6].value = 2;
        // squareDatas[7].value = 2;
        // squareDatas[8].value = 2;
        //
        // squareDatas[0].value = 16;
        // squareDatas[2].value = 16;
        //
        // squareDatas[12].value = 64;
        // var cellCheck = GetSquareDataByCell(new Utils.Cell(1, 1));

        ////
        // squareDatas[0].value = 2;
        // squareDatas[1].value = 8;
        // squareDatas[2].value = 2;
        //
        // squareDatas[6].value = 8;
        // squareDatas[7].value = 2;
        // squareDatas[8].value = 2;
        //
        // squareDatas[12].value = 4;
        // squareDatas[13].value = 8;
        //
        // squareDatas[19].value = 4;

        ////
        squareDatas[2].value = 2;
        squareDatas[8].value = 2;

        var cellCheck = GetSquareDataByCell(new Utils.Cell(2, 1));
        _processingSquare = cellCheck;

        int countActionsList;

        do
        {
            countActionsList = _actionsList.Count;

            // PrinterSquaresData();
            MergeAllBlock();
            SortAllBlock();
        } while (countActionsList < _actionsList.Count);

        Debug.Log("_processingSquare: " + JsonUtility.ToJson(_processingSquare));
        _actionsList.ForEach(e => { Debug.Log(JsonUtility.ToJson(e)); });
    }

    private void Shoot()
    {
        // Debug.Log(JsonUtility.ToJson(MergeBlock(new Utils.Cell(1, 1))));

        // squareDatas.FindAll().ForEach(MergeBlock(new Utils.Cell(1, 1));

        // Debug.Log(JsonUtility.ToJson(SortBlock(new Utils.Cell(1, 1))));
        //   List<MergerAction>  

        // Sequence mySequence = DOTween.Sequence();
        // mySequence.Append(transform.DOMoveX(45, 1))
        //     
        // mySequence.Append(transform.DOMoveX(45, 1))
    }

    private void ResetBoard()
    {
        for (var y = _boardRow; y > 0; y--)
        {
            for (var x = 0; x < _boardCol; x++)
            {
                squareDatas.Add(new SquareData(
                    new Utils.Cell(x, _boardRow - y),
                    x + (_boardRow - y) * _boardCol,
                    0));
            }
        }
    }

    private void MergeAllBlock()
    {
        var squareMergeOrderByCountSameValueList = squareDatas
            .Where(block => block.value > 0)
            .Select(block => new MyClass
            {
                block = block,
                countSquareSameValue = squareDatas
                    .FindAll(squareData =>
                        block.value == squareData.value &&
                        (squareData.index == block.index + 1 ||
                         squareData.index == block.index - 1 ||
                         squareData.index == block.index + _boardCol ||
                         squareData.index == block.index - _boardCol))
                    .Count
            })
            .Where(data => data.countSquareSameValue > 0)
            .OrderByDescending(data => data.countSquareSameValue)
            .ToList();

        squareMergeOrderByCountSameValueList.ForEach(data =>
        {
            if (data.countSquareSameValue == 1)
            {
                MergeBlock(data.block);
            }
            else
            {
                MergeMultiBlock(data.block);
            }
        });
    }

    private void MergeBlock(SquareData dataBlock)
    {
        SquareData squareSource;
        SquareData squareTarget;
        var action = new MergerAction();

        SquareData squareDataSameValue = squareDatas
            .Find(squareData =>
                dataBlock.value == squareData.value &&
                dataBlock.value > 0 &&
                (squareData.index == dataBlock.index + 1 ||
                 squareData.index == dataBlock.index - 1 ||
                 squareData.index == dataBlock.index + _boardCol ||
                 squareData.index == dataBlock.index - _boardCol));

        if (squareDataSameValue is null)
        {
            return;
        }

        var newValue = dataBlock.value * 2;
        var isSameColumn = squareDataSameValue.cell.Row == dataBlock.cell.Row - 1;
        var isSameRowRight = dataBlock.cell.Column > squareDataSameValue.cell.Column &&
                             squareDataSameValue.cell.Column >= _processingSquare.cell.Column;
        var isSameRowLeft = dataBlock.cell.Column < squareDataSameValue.cell.Column &&
                            squareDataSameValue.cell.Column <= _processingSquare.cell.Column;

        if (isSameColumn || isSameRowRight || isSameRowLeft)
        {
            squareSource = dataBlock;
            squareTarget = squareDataSameValue;
        }
        else
        {
            squareSource = squareDataSameValue;
            squareTarget = dataBlock;
        }

        action.squareSources.Add(squareSource);
        action.squareTarget = new SquareData(squareTarget.cell, squareTarget.index,
            squareTarget.value);
        action.newSquareValue = newValue;
        action.type = ActionType.MergeBlock.ToString();

        _actionsList.Add(action);
        squareTarget.value = newValue;
        squareSource.value = 0;
    }

    private void MergeMultiBlock(SquareData cellCheck)
    {
        _count++;
        if (_count > 50)
        {
            print("STACK OVERFLOW");
            return;
        }

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
        action.type = ActionType.MergeMultiBlock.ToString();

        _actionsList.Add(action);

        cellCheck.value = newValue;
    }

    private SquareData GetSquareDataByCell(Utils.Cell cell)
    {
        return squareDatas.Find(item => item.cell.Row == cell.Row && item.cell.Column == cell.Column);
    }

    private static int CountBlockSameValue(
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
        }

        return countBlockSameValue;
    }

    private void SortAllBlock()
    {
        var emptyBlocksUpRowList = squareDatas
            .FindAll(item => item.value == 0 &&
                             squareDatas.Any(squareDownRow =>
                                 squareDownRow.index == item.index + _boardCol &&
                                 squareDownRow.value > 0));

        if (!emptyBlocksUpRowList.Any())
        {
            return;
        }

        emptyBlocksUpRowList.ForEach(emptyBlocksUpRow =>
        {
            var squareHasValueDownRowList = GetSquaresDataHasValueDownRowByCell(emptyBlocksUpRow);
            squareHasValueDownRowList.ForEach(squareHasValueDownRow =>
            {
                SortBlock(squareHasValueDownRow.cell,
                    new Utils.Cell(squareHasValueDownRow.cell.Column, squareHasValueDownRow.cell.Row - 1));
            });
        });
    }

    private void SortAllBlock2()
    {
        var squareHasEmptyBlocksUpRow = squareDatas.Find(item =>
            item.value > 0 && squareDatas.Any(squareDownRow =>
                squareDownRow.index == item.index - _boardCol && squareDownRow.value == 0));

        if (squareHasEmptyBlocksUpRow == null)
        {
            return;
        }

        var squareEmptyUpRow = GetSquareDataEmptyUpRowByCell(squareHasEmptyBlocksUpRow.cell);

        if (squareEmptyUpRow == null)
        {
            return;
        }

        SortBlock(squareHasEmptyBlocksUpRow.cell, squareEmptyUpRow.cell);
    }

    private void SortBlock(Utils.Cell cellCheck, Utils.Cell cellUp)
    {
        var action = new MergerAction();
        var squareUp = GetSquareDataByCell(cellUp);
        var squareCheck = GetSquareDataByCell(cellCheck);

        action.squareSources.Add(new SquareData(squareCheck.cell, squareCheck.index, squareCheck.value));
        action.squareTarget = new SquareData(squareUp.cell, squareUp.index, squareUp.value);
        action.newSquareValue = squareCheck.value;
        action.type = ActionType.SortBlock.ToString();

        _actionsList.Add(action);

        squareUp.value = squareCheck.value;
        squareCheck.value = 0;

        if (squareCheck == _processingSquare)
        {
            _processingSquare = squareUp;
        }
    }

    private List<SquareData> GetSquaresDataHasValueDownRowByCell(SquareData squareEmptyUpRow)
    {
        return squareDatas.FindAll(squareData => squareData.cell.Column == squareEmptyUpRow.cell.Column &&
                                                 squareData.value > 0 &&
                                                 squareData.cell.Row > squareEmptyUpRow.cell.Row)
            .OrderBy(squareData => squareData.index)
            .ToList();
    }

    private SquareData GetSquareDataEmptyUpRowByCell(Utils.Cell cell)
    {
        SquareData result = null;
        for (int cellRow = cell.Row - 1; cellRow >= 0; cellRow--)
        {
            var squareDataEmpty = squareDatas.Find(item =>
                item.cell.Row == cellRow && item.cell.Column == cell.Column && item.value == 0);

            if (squareDataEmpty == null)
            {
                return result;
            }

            result = squareDataEmpty;
        }

        return result;
    }

    private void PrinterSquaresData()
    {
        print("__________");
        squareDatas
            .FindAll(e => e.value > 0)
            .ForEach(e => { Debug.Log(JsonUtility.ToJson(e)); });
    }

    #endregion
}

public class MyClass
{
    public int countSquareSameValue;
    public SquareData block;
}