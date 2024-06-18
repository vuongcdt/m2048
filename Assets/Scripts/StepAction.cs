using System;
using System.Collections.Generic;

[Serializable]
public class StepAction
{
    public List<SquareData> squareSources = new();
    public SquareData squareTarget;
    public int newSquareValue;
}
public class BoardAction
{
    public ActionType actionType;
    public List<StepAction> stepActionList;

    public BoardAction(List<StepAction> stepActionList, ActionType actionType)
    {
        this.stepActionList = stepActionList;
        this.actionType = actionType;
    }
}

public enum ActionType
{
    None,
    Shoot,
    MergeAllBlock,
    SortAllBlock,
    ClearMinBlock
}