using System;
using UnityEngine;

[Serializable]
public class SquareData
{
    public Utils.Cell cell;
    public int id;
    public int value;

    public Vector2 Position => Utils.GridToPos(cell.Row, cell.Column);

    public SquareData(Utils.Cell cell, int id, int value)
    {
        this.cell = cell;
        this.id = id;
        this.value = value;
    }
}