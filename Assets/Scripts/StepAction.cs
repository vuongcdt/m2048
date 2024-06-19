using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

[Serializable]
public class StepAction
{
    public List<SquareData> multiSquareSources = new();
    public SquareData singleSquareSources;
    public SquareData squareTarget;
    public float newSquareValue;
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