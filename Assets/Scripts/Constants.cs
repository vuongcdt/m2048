using UnityEngine;

public class Constants
{
    public class PrefConsts
    {
        public const string SQUARES_DATA_KEY = "BoardKey";
        public const string SCORE_KEY = "ScoreKey";
        public const string HIGH_SCORE_KEY = "HighScoreKey";
        public const string ID_COUNT_KEY = "IdCountKey";
        public const string NEXT_SQUARE_VALUE_KEY = "NextSquareValueKey";
    }

    public class SquareColor
    {
        public static readonly Color White = Color.white;
        public static readonly Color Red = Color.red;
        public static readonly Color Blue = Color.blue;
        public static readonly Color Green = Color.green;
        public static readonly Color Yellow = Color.yellow;
        public static readonly Color Magenta = Color.magenta;
        public static readonly Color Cyan = Color.cyan;
        public static readonly Color Grey = Color.grey;
    }

    public class TimeMove
    {
        public const float TimeSquareMoveToPoint = 0.3f;
    }
}