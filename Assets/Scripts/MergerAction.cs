using System;
using System.Collections.Generic;

[Serializable]
public class MergerAction
{
    public string type;
    public List<SquareData> squareSources = new();
    public SquareData squareTarget;
    public int newSquareValue;
}

public enum ActionType
{
    MergeBlock,
    MergeMultiBlock,
    SortBlock
}