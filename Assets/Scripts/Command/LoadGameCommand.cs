using System.Collections.Generic;
using UnityEngine;

public class LoadGameCommand : CommandBase
{
    private List<float> _squareValueList = new();
    private BoardManager _boardManager;
    private UIManager _uiManager;

    public LoadGameCommand(List<float> squareValueList)
    {
        _squareValueList = squareValueList;
        _boardManager = BoardManager.Instance;
        _uiManager = UIManager.Instance;
    }
    protected override void Init()
    {
    }

    public override void Excute()
    {
        LoadDataFromPrefs();
    }

    private void LoadDataFromPrefs()
    {
        // RenderBoard();
        var renderBoardCommand = new RenderBoardCommand();
        renderBoardCommand.Excute();

        if (!string.IsNullOrEmpty(Prefs.SquaresData))
        {
            _boardManager.squaresData = JsonUtility.FromJson<Utils.JsonHelper<SquareData>>(Prefs.SquaresData).data;
        }

        _boardManager.score = Prefs.Score;
        _boardManager.idCount = Prefs.IdCount == 0 ? 30 : Prefs.IdCount;
        _uiManager.idCount = _boardManager.idCount;

        LoadHighScore();
        LoadSquareValueList();

        _boardManager.nextSquareValue = Prefs.NextSquareValue;
    }

    private void LoadSquareValueList()
    {
        if (string.IsNullOrWhiteSpace(Prefs.SquareValueList) || string.IsNullOrEmpty(Prefs.SquareValueList))
        {
            return;
        }

        var numsList = JsonUtility.FromJson<Utils.JsonHelper<float>>(Prefs.SquareValueList).data;
        var valueListPrefs = new List<float>();
        foreach (var value in numsList)
        {
            if (value > 0)
            {
                valueListPrefs.Add(value);
            }
        }


        if (valueListPrefs.Count > 0)
        {
            _squareValueList = valueListPrefs;
        }
    }

    private void LoadHighScore()
    {
        _boardManager.highScore = Prefs.HighScore;
    }
}
