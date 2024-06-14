using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

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
        //
        // MergeBlock(new Utils.Cell(1, 1));
        
        squareDatas[0].value = 2;
        squareDatas[1].value = 8;
        squareDatas[2].value = 2; 
        // squareDatas[2].value = 4; 
        squareDatas[6].value = 8;
        squareDatas[7].value = 2;
        squareDatas[8].value = 2;
        squareDatas[12].value = 4;
        squareDatas[13].value = 8;
        squareDatas[19].value = 4;
        
        MergeBlock(new Utils.Cell(2, 1));

        _actionsList.ForEach(e => { Debug.Log(JsonUtility.ToJson(e)); });
    }

    private void Shoot()
    {
        // Debug.Log(JsonUtility.ToJson(MergeBlock(new Utils.Cell(1, 1))));

        // _squareDatas.FindAll().ForEach(MergeBlock(new Utils.Cell(1, 1));

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

    private void MergeBlock(Utils.Cell cell)
    {
        _count++;
        if (_count > 8)
        {
            Debug.Log("stack overflow");
            return;
        }

        var action = new MergerAction();
        var countBlockSameValue = 0;

        var cellCheck = GetSquareDataByCell(new Utils.Cell(cell.Column, cell.Row));
        _processingSquare = cellCheck;

        var squareLeft = GetSquareDataByCell(new Utils.Cell(cell.Column - 1, cell.Row));
        var squareUp = GetSquareDataByCell(new Utils.Cell(cell.Column, cell.Row - 1));
        var squareRight = GetSquareDataByCell(new Utils.Cell(cell.Column + 1, cell.Row));

        countBlockSameValue = CountBlockSameValue(squareLeft, cellCheck, countBlockSameValue, action);
        countBlockSameValue = CountBlockSameValue(squareUp, cellCheck, countBlockSameValue, action);
        countBlockSameValue = CountBlockSameValue(squareRight, cellCheck, countBlockSameValue, action);

        if (countBlockSameValue == 0)
        {
            SortAllBlock();
            return;
        }

        action.squareTarget = new SquareData(cellCheck.cell, cellCheck.index, cellCheck.value);

        var powSquares = cellCheck.value * (int)Mathf.Pow(2, countBlockSameValue);

        action.newSquareValue = powSquares;
        action.type = ActionType.MergeBlock.ToString();

        _actionsList.Add(action);

        cellCheck.value = powSquares;
        
        MergeAllBlock();
        
        if (squareUp != null)
        {
            SortBlock(cellCheck, squareUp);
        }

        SortAllBlock();
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

    private void SortBlock(SquareData cellCheck, SquareData squareUp)
    {
        var action = new MergerAction();

        action.squareSources.Add(new SquareData(cellCheck.cell, cellCheck.index, cellCheck.value));
        action.squareTarget = new SquareData(squareUp.cell, squareUp.index, squareUp.value);
        action.newSquareValue = cellCheck.value;
        action.type = ActionType.SortBlock.ToString();

        _actionsList.Add(action);

        squareUp.value = cellCheck.value;
        cellCheck.value = 0;
        
        if (cellCheck == _processingSquare)
        {
            _processingSquare = squareUp;
        }
        else
        {
            MergeBlock(_processingSquare.cell);
        }
        
        MergeBlock(squareUp.cell);
    }

    private void SortAllBlock()
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

        SortBlock(squareHasEmptyBlocksUpRow, squareEmptyUpRow);
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

    private void MergeAllBlock()
    {
        
    }
    #endregion
}