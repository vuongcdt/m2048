using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NextSquareCommand : CommandBase<bool>
{
    private List<SquareData> _squaresData = new();
    private SquareData _processingSquare;
    private List<StepAction> _actionsList = new();
    private List<BoardAction> _actionsWrapList = new();
    private List<float> _squareValueList = new();
    private readonly int[] _probabilityList = { 1, 4, 10, 18, 28, 40, 54, 70, 88 };
    private const int MAX_COUNT_QUARE_VALUE_LIST = 9;
    private BoardManager _boardManager;

    public NextSquareCommand(List<SquareData> squaresData, List<float> squareValueList, SquareData processingSquare, List<StepAction> actionsList, List<BoardAction> actionsWrapList)
    {
        _squaresData = squaresData;
        _actionsList = actionsList;
        _actionsWrapList = actionsWrapList;
        _processingSquare = processingSquare;
        _squareValueList = squareValueList;
        _boardManager = BoardManager.Instance;
    }
    protected override void Init()
    {
    }

    public override bool Excute()
    {
        SetRandomSquareValue();
        return true;
    }


    private void SetRandomSquareValue()
    {
        SetNewValueInSquareValueList();
        SetRandomValue();
    }

    private void SetRandomValue()
    {
        var countValueList = _squareValueList.Count;
        List<int> probabilityList = new();
        for (var i = 0; i < countValueList; i++)
        {
            probabilityList.Add(_probabilityList[i]);
        }

        var maxValue = probabilityList[^1];
        var randomNum = Random.Range(0, 1f) * maxValue;

        for (var i = 0; i < countValueList; i++)
        {
            if (probabilityList[i] > randomNum)
            {
                var value = _squareValueList[countValueList - 1 - i];
                _boardManager.nextSquareValue = value;
                Observer.Emit(Constants.EventKey.NEXT_SQUARE, value);

                return;
            }
        }
    }

    private void SetNewValueInSquareValueList()
    {
        var maxValueInBoard = _boardManager.squaresData.AsEnumerable().Max(square => square.value);
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

    private void ClearMinBlock(float minValueInBoard)
    {
        _actionsList.Clear();

        foreach (var squareData in _boardManager.squaresData)
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

        _boardManager.ProcessingLoop();
    }
}
