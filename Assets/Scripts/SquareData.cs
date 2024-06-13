using System;

[Serializable]
public class SquareData
{
    public int row;
    public int column;
    public int index;
    public int value;

    public SquareData(int row, int column, int index, int value)
    {
        this.row = row;
        this.column = column;
        this.index = index;
        this.value = value;
    }
}