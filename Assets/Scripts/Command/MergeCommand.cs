using System.Collections.Generic;
using UnityEngine;

public class MergeCommand : CommandBase<bool>
{
    private List<SquareData> _squaresData = new();
    private SquareData _processingSquare;
    private List<StepAction> _actionsList = new();
    private List<BoardAction> _actionsWrapList = new();

    public MergeCommand(List<SquareData> squaresData, SquareData processingSquare, List<StepAction> actionsList, List<BoardAction> actionsWrapList)
    {
        _squaresData = squaresData;
        _actionsList = actionsList;
        _actionsWrapList = actionsWrapList;
        _processingSquare = processingSquare;
    }

    public override bool Excute()
    {
        MergeAllBlock();
        return true;
    }

    protected override void Init()
    {
    }

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
        foreach (var block in _squaresData)
        {
            if (!(block.value > 0))
            {
                continue;
            }

            List<SquareData> squareSameValueList = new();
            GetCountSquareSameValueList(block, squareSameValueList);

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
        foreach (var squareData in _squaresData)
        {
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

        var countBlockSameValue = squareSourceList.Count;

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
}
