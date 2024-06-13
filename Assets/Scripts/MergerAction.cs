using System;
using System.Collections.Generic;

[Serializable]
public class MergerAction
{
    public List<SquareData> squareSources = new();
    public SquareData squareTarget;
    public int newSquareValue;
}