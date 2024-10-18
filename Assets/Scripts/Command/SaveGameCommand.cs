using System.Collections.Generic;
using UnityEngine;

public class SaveGameCommand : CommandBase<bool>
{
    private List<float> _squareValueList = new();
    private bool _isSave;
    private BoardManager _boardManager;

    public SaveGameCommand(List<float> squareValueList, bool isSave)
    {
        _squareValueList = squareValueList;
        _isSave = isSave;
        _boardManager = BoardManager.Instance;
    }

    protected override void Init()
    {
    }

    public override bool Excute()
    {
        SaveGame();
        return true;
    }

    private void SaveGame()
    {
        if (_isSave)
        {
            return;
        }

        _isSave = true;

        var jsonHelperSquaresData = new Utils.JsonHelper<SquareData>(_boardManager.squaresData);
        var jsonHelperValueList = new Utils.JsonHelper<float>(_squareValueList);

        Prefs.SquaresData = JsonUtility.ToJson(jsonHelperSquaresData);
        Prefs.Score = _boardManager.score;
        Prefs.HighScore = _boardManager.highScore;
        Prefs.IdCount = _boardManager.idCount;
        Prefs.SquareValueList = JsonUtility.ToJson(jsonHelperValueList);
        Prefs.NextSquareValue = _boardManager.nextSquareValue;
    }

}
