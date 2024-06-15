using System;
using System.Collections.Generic;

[Serializable]
public class MergerAction
{
    public List<SquareData> squareSources = new();
    public SquareData squareTarget;
    public int newSquareValue;
}
public class MergerActionWrap
{
    public List<MergerAction> mergerActionList;
    public ActionType actionType;

    public MergerActionWrap(List<MergerAction> mergerActionList, ActionType actionType)
    {
        this.mergerActionList = mergerActionList;
        this.actionType = actionType;
    }
}

public enum ActionType
{
    None,
    Shoot,
    MergeAllBlock,
    SortAllBlock,
}