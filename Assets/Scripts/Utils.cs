using System;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    private static List<Color32> _colors = new()
    {
        new(22, 160, 133, 255),
        new(41, 128, 185, 255),
        new(142, 68, 173, 255),
        new(243, 156, 18, 255),
        new(211, 84, 0, 255),
        new(192, 57, 43, 255),
        new(39, 174, 96, 255),
        new(232, 67, 147, 255),
        new(116, 185, 255, 255),
        new(162, 155, 254, 255),
    };

    public static Vector2 GridToPos(int row, int col)
    {
        return new Vector2(-5 + col * 2, 4 - row * 2);
    }

    public static Cell PosToGrid(Vector2 pos)
    {
        return new Cell((int)((pos.x + 5) / 2), (int)((4 - pos.y) / 2));
    }

    public static string GetText(float value)
    {
        var valueFormat = value.ToString();
        switch (value)
        {
            case >= 1024 * 1024 * 1024:
                valueFormat = value / (1024 * 1024 * 1024) + "B";
                break;
            case >= 1024 * 1024:
                valueFormat = value / (1024 * 1024) + "M";
                break;
            case >= 1024 * 8:
                valueFormat = value / 1024 + "K";
                break;
        }

        return valueFormat;
    }

    public static Color GetColor(float value)
    {
        var exponent = GetExponent(value);

        var colorIndex = exponent % 10;
        return _colors[colorIndex];
    }

    public static int GetExponent(float value)
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

    public class JsonHelper<T>
    {
        public List<T> data;

        public JsonHelper(List<T> data)
        {
            this.data = data;
        }  
        public JsonHelper()
        {
        }
    }
    
    public class RankData
    {
        public string dateTime;
        public List<ChartScore> chartScores;
        public float highScore;

        public RankData(List<ChartScore> chartScores, string dateTime, float highScore)
        {
            this.chartScores = chartScores;
            this.dateTime = dateTime;
            this.highScore = highScore;
        }
    }

    public class CountSquareList
    {
        public SquareData square;
        public List<SquareData> squareSameValueList;

        public CountSquareList(SquareData square, List<SquareData> squareSameValueList)
        {
            this.square = square;
            this.squareSameValueList = squareSameValueList;
        }
    }

    [Serializable]
    public class ChartScore
    {
        public int index;
        public float score;
        public string fullName;

        public ChartScore(float score, string fullName)
        {
            this.score = score;
            this.fullName = fullName;
        }
    }
}