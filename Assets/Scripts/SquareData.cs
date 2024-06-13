using System;
using UnityEngine;

[Serializable]
public class SquareData
{
    public int row;
    public int column;
    public int index;
    public int value;

    public Vector2 Position => Utils.GridToPos(this.row, this.column);

    public SquareData(int row, int column, int index, int value)
    {
        this.row = row;
        this.column = column;
        this.index = index;
        this.value = value;
    }

    public SquareData(Square square)
    {
        this.row = square.row;
        this.column = square.column;
        this.index = square.index;
        this.value = square.value;
    }
}