using System;
using UnityEngine;

[Serializable]
public class SquareData
{
    public Utils.Cell cell;
    public int index;
    public int value;

    public Vector2 Position => Utils.GridToPos(cell.Row,cell.Column);

    public SquareData(Utils.Cell cell, int index, int value)
    {
        this.cell = cell;
        this.index = index;
        this.value = value;
    }
}