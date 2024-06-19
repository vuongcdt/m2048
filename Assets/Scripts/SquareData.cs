using System;
using UnityEngine;

[Serializable]
public class SquareData
{
    public Utils.Cell cell;
    public int id;
    public float value;
    

    public Vector2 Position => Utils.GridToPos(cell.Row, cell.Column);

    public SquareData(Utils.Cell cell, int id, float value)
    {
        this.cell = cell;
        this.id = id;
        this.value = value;
    }
}