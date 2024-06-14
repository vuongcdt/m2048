using System;
using System.Collections.Generic;

[Serializable]
public class MergerAction
{
    public ActionType type;
    public List<SquareData> squareSources = new();
    public SquareData squareTarget;
    public int newSquareValue;
}
[Serializable]
public enum ActionType
{
    None,
    Shoot,
    MergeBlock,
    MergeMultiBlock,
    SortBlock,
}