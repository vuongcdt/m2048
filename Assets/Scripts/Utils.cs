using System;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static Vector2 GridToPos(int row, int col)
    {
        return new Vector2(-5 + col * 2, 4 - row * 2);
    }

    public static Cell PosToGrid(Vector2 pos)
    {
        return new Cell((int)((pos.x + 5) / 2), (int)((4- pos.y) / 2));
    }

    public static int GetExponent(long value)
    {
        return (int)(Mathf.Log(value) / Mathf.Log(2));
    }

    [Serializable]
    public class Cell
    {
        public int Row;
        public int Column;

        public Cell(int column, int row)
        {
            Row = row;
            Column = column;
        }
    }
    
    
    public class JsonHelper
    {
        public List<SquareData> datas;

        public JsonHelper(List<SquareData> datas)
        {
            this.datas = datas;
        }
    }
}