using System.Collections.Generic;
using UnityEngine;

public class ShootCommand : CommandBase
{
    private List<SquareData> _squaresData = new();
    private List<StepAction> _actionsList = new();
    private List<BoardAction> _actionsWrapList = new();
    private int _columnSelect;
    public float _nextSquareValue;
    private BoardManager _boardManager;
    public int _boardRow;

    public ShootCommand(List<SquareData> squaresData, List<StepAction> actionsList,
     List<BoardAction> actionsWrapList, int columnSelect, float nextSquareValue, int boardRow)
    {
        _squaresData = squaresData;
        _actionsList = actionsList;
        _actionsWrapList = actionsWrapList;
        _columnSelect = columnSelect;
        _nextSquareValue = nextSquareValue;
        _boardRow = boardRow;
        _boardManager = BoardManager.Instance;
    }

    public override void Excute()
    {
        Shoot(_columnSelect);
    }

    protected override void Init()
    {
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

        _boardManager.idCount++;
        var squareSource = new SquareData(
            new Utils.Cell(column, _boardRow),
            _boardManager.idCount,
            _nextSquareValue
        );

        action.squareTarget = new SquareData(squareTarget.cell, squareTarget.id, squareTarget.value);
        action.singleSquareSources = new SquareData(squareSource.cell, squareSource.id, squareSource.value);
        action.newSquareValue = _nextSquareValue;

        _actionsList.Add(action);
        var item = new BoardAction(new List<StepAction>(_actionsList), ActionType.Shoot);
        _actionsWrapList.Add(item);

        squareTarget.id = squareSource.id;
        squareSource.id = 0;
        squareTarget.value = _nextSquareValue;
        _boardManager.processingSquare = squareTarget;
    }

    private void CheckMergeWhenMaxItemColumn(int column, StepAction action)
    {
        foreach (var squareTarget in _squaresData)
        {
            if (Mathf.Approximately(squareTarget.value, _nextSquareValue) &&
                squareTarget.cell.Column == column &&
                squareTarget.cell.Row == _boardRow - 1)
            {
                _boardManager.idCount++;
                var squareSource = new SquareData(
                    new Utils.Cell(column, _boardRow),
                    _boardManager.idCount,
                    _nextSquareValue
                );

                var newSquareValue = _nextSquareValue * 2;
                action.squareTarget = new SquareData(squareTarget.cell, squareTarget.id, squareTarget.value);
                action.multiSquareSources.Add(new SquareData(squareSource.cell, squareSource.id, squareSource.value));
                action.newSquareValue = newSquareValue;

                _actionsList.Add(action);
                var item = new BoardAction(new List<StepAction>(_actionsList), ActionType.MergeAllBlock);
                _actionsWrapList.Add(item);
                squareTarget.value = newSquareValue;
                _boardManager.processingSquare = squareTarget;

                return;
            }
        }
    }

    private SquareData GetEmptySquareDataTargetByColumn(int column)
    {
        foreach (var block in _squaresData)
        {
            if (block.value == 0 && block.cell.Column == column)
            {
                return block;
            }
        }

        return null;
    }

}
