using System.Collections.Generic;
using UnityEngine;

public class SortCommand : CommandBase
{
    private List<SquareData> _squaresData = new();
    private SquareData _processingSquare;
    private List<StepAction> _actionsList = new();
    private List<BoardAction> _actionsWrapList = new();
    private int _boardCol;

    public SortCommand(List<SquareData> squaresData, SquareData processingSquare, List<StepAction> actionsList, List<BoardAction> actionsWrapList, int boardCol)
    {
        _squaresData = squaresData;
        _actionsList = actionsList;
        _actionsWrapList = actionsWrapList;
        _processingSquare = processingSquare;
        _boardCol = boardCol;
    }

    public override void Excute()
    {
        SortAllBlock();
    }

    protected override void Init()
    {
    }

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

    private List<SquareData> GetEmptyBlocksUpRowList()
    {
        var emptyBlocksUpRowList = new List<SquareData>();
        foreach (var item in _squaresData)
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
        foreach (var squareDownRow in _squaresData)
        {
            var isUpRow = squareDownRow.cell.Row == item.cell.Row + 1;
            var isSameColumn = squareDownRow.cell.Column == item.cell.Column;
            if (isUpRow && isSameColumn && squareDownRow.value > 0) return true;
        }

        return false;
    }

    private void SortBlockBySquaresDataHasValueDownRowByCell(SquareData squareEmptyUpRow)
    {
        foreach (SquareData squareData in _squaresData)
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
        foreach (var squareData in _squaresData)
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
        return squareSource.cell.Column + squareSource.cell.Row * _boardCol + 1;
    }
}
