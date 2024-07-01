using UnityEngine;

public static class Prefs
{
    public static string SquaresData
    {
        get => PlayerPrefs.GetString(Constants.PrefConsts.SQUARES_DATA_KEY);
        set => PlayerPrefs.SetString(Constants.PrefConsts.SQUARES_DATA_KEY, value);
    }

    public static string SquareValueList
    {
        get => PlayerPrefs.GetString(Constants.PrefConsts.SQUARE_VALUE_LIST_KEY);
        set => PlayerPrefs.SetString(Constants.PrefConsts.SQUARE_VALUE_LIST_KEY, value);
    }

    public static string RankData
    {
        get => PlayerPrefs.GetString(Constants.PrefConsts.RANK_KEY);
        set => PlayerPrefs.SetString(Constants.PrefConsts.RANK_KEY, value);
    }

    public static int IdCount
    {
        get => PlayerPrefs.GetInt(Constants.PrefConsts.ID_COUNT_KEY);
        set => PlayerPrefs.SetInt(Constants.PrefConsts.ID_COUNT_KEY, value);
    }

    public static float Score
    {
        get => PlayerPrefs.GetFloat(Constants.PrefConsts.SCORE_KEY);
        set => PlayerPrefs.SetFloat(Constants.PrefConsts.SCORE_KEY, value);
    }

    public static float HighScore
    {
        get => PlayerPrefs.GetFloat(Constants.PrefConsts.HIGH_SCORE_KEY);
        set => PlayerPrefs.SetFloat(Constants.PrefConsts.HIGH_SCORE_KEY, value);
    }

    public static float NextSquareValue
    {
        get => PlayerPrefs.GetFloat(Constants.PrefConsts.NEXT_SQUARE_VALUE_KEY);
        set => PlayerPrefs.SetFloat(Constants.PrefConsts.NEXT_SQUARE_VALUE_KEY, value);
    }

    public static float VolumeMusic
    {
        get => PlayerPrefs.GetFloat(Constants.PrefConsts.VOLUMME_MUSIC,-1);
        set => PlayerPrefs.SetFloat(Constants.PrefConsts.VOLUMME_MUSIC, value);
    }

    public static float VolumeSfx
    {
        get => PlayerPrefs.GetFloat(Constants.PrefConsts.VOLUMME_SFX,-1);
        set => PlayerPrefs.SetFloat(Constants.PrefConsts.VOLUMME_SFX, value);
    }

    public static void ClearPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}