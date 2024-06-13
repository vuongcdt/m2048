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

    public class Cell
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public Cell(int column, int row)
        {
            Row = row;
            Column = column;
        }
    }
}