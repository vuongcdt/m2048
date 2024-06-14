using System;
using System.Collections.Generic;

[Serializable]
public class MergerAction
{
    public List<SquareData> squareSources = new();
    public SquareData squareTarget;
    public int newSquareValue;
    public string type;
}

public enum ActionType
{
    MergeBlock,
    SortBlock
}