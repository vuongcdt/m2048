using UnityEngine;

public class Constants
{
    public class PrefConsts
    {
        public const string SQUARES_DATA_KEY = nameof(SQUARES_DATA_KEY);
        public const string SCORE_KEY = nameof(SCORE_KEY);
        public const string HIGH_SCORE_KEY = nameof(HIGH_SCORE_KEY);
        public const string ID_COUNT_KEY = nameof(ID_COUNT_KEY);
        public const string NEXT_SQUARE_VALUE_KEY = nameof(NEXT_SQUARE_VALUE_KEY);
        public const string VOLUMME_MUSIC = nameof(VOLUMME_MUSIC);
        public const string VOLUMME_SFX = nameof(VOLUMME_SFX);
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