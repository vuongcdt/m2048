using UnityEngine;

public static class Prefs
{
    public static string SquaresData
    {
        get => PlayerPrefs.GetString(Constants.PrefConsts.SQUARES_DATA_KEY,"");
        set => PlayerPrefs.SetString(Constants.PrefConsts.SQUARES_DATA_KEY, value);
    }
    public static string Score
    {
        get => PlayerPrefs.GetString(Constants.PrefConsts.SCORE_KEY);
        set => PlayerPrefs.SetString(Constants.PrefConsts.SCORE_KEY, value);
    }
    public static string HighScore
    {
        get => PlayerPrefs.GetString(Constants.PrefConsts.HIGH_SCORE_KEY);
        set => PlayerPrefs.SetString(Constants.PrefConsts.HIGH_SCORE_KEY, value);
    }
    public static int IdCount
    {
        get => PlayerPrefs.GetInt(Constants.PrefConsts.ID_COUNT_KEY);
        set => PlayerPrefs.SetInt(Constants.PrefConsts.ID_COUNT_KEY, value);
    }
    public static string NextSquareValue
    {
        get => PlayerPrefs.GetString(Constants.PrefConsts.NEXT_SQUARE_VALUE_KEY);
        set => PlayerPrefs.SetString(Constants.PrefConsts.NEXT_SQUARE_VALUE_KEY, value);
    }
    public static string SquareValueList
    {
        get => PlayerPrefs.GetString(Constants.PrefConsts.NEXT_SQUARE_VALUE_KEY);
        set => PlayerPrefs.SetString(Constants.PrefConsts.NEXT_SQUARE_VALUE_KEY, value);
    }
    public static void ClearPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}