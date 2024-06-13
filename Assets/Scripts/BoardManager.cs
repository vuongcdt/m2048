using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardManager : Singleton<BoardManager>
{
    #region NewCode

    [SerializeField] private List<SquareData> _squareDatas = new();
    [SerializeField] private List<Square> _squares = new();

    private int boardRow = 5;
    private int boardCol = 6;

    private void Start()
    {
        ResetBoard();

        _squareDatas[1].value = 2;
        _squareDatas[6].value = 2;
        _squareDatas[7].value = 2;
        _squareDatas[8].value = 2;

        Debug.Log(JsonUtility.ToJson(MergeBlock(new Utils.Cell(1, 1))));
        Debug.Log(JsonUtility.ToJson(SortBlock(new Utils.Cell(1, 1))));
        Debug.Log(JsonUtility.ToJson(SortAllBlock()));
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
        for (var y = boardRow; y > 0; y--)
        {
            for (var x = 0; x < boardCol; x++)
            {
                _squareDatas.Add(new SquareData(
                    new Utils.Cell(x, boardRow - y),
                    x + (boardRow - y) * boardCol,
                    0));
            }
        }
    }

    private MergerAction MergeBlock(Utils.Cell cell)
    {
        var action = new MergerAction();
        var countBlockSameValue = 1;

        var cellCheck = GetSquareDataByCell(new Utils.Cell(cell.Column, cell.Row));

        var squareLeft = GetSquareDataByCell(new Utils.Cell(cell.Column - 1, cell.Row));
        var squareUp = GetSquareDataByCell(new Utils.Cell(cell.Column, cell.Row - 1));
        var squareRight = GetSquareDataByCell(new Utils.Cell(cell.Column + 1, cell.Row));

        // Debug.Log($"squareLeft {JsonUtility.ToJson(squareLeft)}");
        // Debug.Log($"cellCheck {JsonUtility.ToJson(cellCheck)}");

        countBlockSameValue = CountBlockSameValue(squareLeft, cellCheck, countBlockSameValue, action);
        countBlockSameValue = CountBlockSameValue(squareUp, cellCheck, countBlockSameValue, action);
        countBlockSameValue = CountBlockSameValue(squareRight, cellCheck, countBlockSameValue, action);

        action.squareTarget = new SquareData(cellCheck.cell, cellCheck.index, cellCheck.value);
        action.newSquareValue = (int)Mathf.Pow(cellCheck.value, countBlockSameValue);
        cellCheck.value = action.newSquareValue;

        return action;
    }

    private static int CountBlockSameValue(
        SquareData squareData,
        SquareData cellCheck,
        int countBlockSameValue,
        MergerAction action)
    {
        if (squareData != null && squareData.value == cellCheck.value)
        {
            action.squareSources.Add(new SquareData(squareData.cell, squareData.index, squareData.value));
            squareData.value = 0;
            countBlockSameValue++;
        }

        return countBlockSameValue;
    }

    private List<MergerAction> SortBlock(Utils.Cell cell)
    {
        var actionList = new List<MergerAction>();
        var action = new MergerAction();
        
        var cellCheck = GetSquareDataByCell(new Utils.Cell(cell.Column, cell.Row));
        var squareUp = GetSquareDataByCell(new Utils.Cell(cell.Column, cell.Row - 1));

        if (squareUp is { value: 0 })
        {
            action.squareSources.Add(cellCheck);
            action.squareTarget = squareUp;
            action.newSquareValue = cellCheck.value;

            squareUp.value = cellCheck.value;
            cellCheck.value = 0;
            actionList.Add(action);
            
            MergerAction mergerAction = MergeBlock(squareUp.cell);
            actionList.Add(mergerAction);
        }

        return actionList;
    }

    private List<MergerAction> SortAllBlock()
    {
        var actionList = new List<MergerAction>();

        var emptyBlocks =
            _squareDatas.Find(item =>
                item.value == 0
                && _squareDatas.Any(squareDownRow => squareDownRow.index == item.index + 6 && squareDownRow.value > 0));

        if (emptyBlocks != null)
        {
            var sortActions = SortBlock(emptyBlocks.cell);
            actionList.AddRange(sortActions);
        }

        return actionList;
    }

    private SquareData GetSquareDataByCell(Utils.Cell cell)
    {
        return _squareDatas.Find(item => item.cell.Row == cell.Row && item.cell.Column == cell.Column);
    }

    #endregion
}