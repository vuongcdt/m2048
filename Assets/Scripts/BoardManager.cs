using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoardManager : Singleton<BoardManager>
{
    [SerializeField] private GameObject lineColumn;
    [SerializeField] private Transform lineParentTransform;

    public List<SquareData> squaresData = new();
    public int boardRow = 5;
    public int boardCol = 6;
    public bool isTouchLine;
    public int columnSelect;
    public bool isProcessing;
    public float score;
    public float highScore;
    public float nextSquareValue;
    public bool isGameOver;
    public bool isPlaying;
    public int idCount;
    public SquareData processingSquare;

    private bool _isSave;
    private UIManager _uiManager;
    private SoundManager _soundManager;
    private List<GameObject> _lineColumnList = new();
    private List<StepAction> _actionsList = new();
    private List<BoardAction> _actionsWrapList = new();
    private List<float> _squareValueList = new() { 2, 4 };

    private void Start()
    {
        Application.targetFrameRate = 60;
        _uiManager = UIManager.Instance;
        _soundManager = SoundManager.Instance;

        RenderLineColumn();

        var loadGameCommand = new LoadGameCommand(_squareValueList);
        loadGameCommand.Excute();

        CheckGameOver();
        if (isGameOver)
        {
            _uiManager.ResetGameUI();
            RestartGame();
            isProcessing = false;
            isGameOver = false;
        }

        // TestData.SetDataTest(squaresData);

        if (nextSquareValue == 0)
        {
            nextSquareValue = 2;
        }

        _uiManager.StartUI(squaresData);
    }

    private void RenderLineColumn()
    {
        for (int i = 0; i < boardCol; i++)
        {
            var posLine = new Vector2(i * 2 - boardRow, 0);
            var line = Instantiate(lineColumn, posLine, Quaternion.identity, lineParentTransform);

            line.GetComponent<LineColumn>().column = i;

            _lineColumnList.Add(line);
        }
    }

    public void RestartGame()
    {
        idCount = 0;
        var renderBoardCommand = new RenderBoardCommand();
        renderBoardCommand.Excute();

        score = 0;
        LoadHighScore();
        _squareValueList.Clear();
        _squareValueList.Add(2);

        var randomSquareCommand = new NextSquareCommand(squaresData, _squareValueList, processingSquare, _actionsList, _actionsWrapList);
        randomSquareCommand.Excute();
    }

    public IEnumerator ShootBlock()
    {
        isProcessing = true;
        _actionsWrapList.Clear();
        yield return new WaitForNextFrameUnit();

        ProcessingData(columnSelect);

        if (_actionsWrapList.Count <= 0)
        {
            yield return null;
        }

        yield return new WaitForNextFrameUnit();

        if (_actionsWrapList.Count > 0)
        {
            var randomSquareCommand = new NextSquareCommand(squaresData, _squareValueList, processingSquare, _actionsList, _actionsWrapList);
            randomSquareCommand.Excute();
        }

        CheckGameOver();

        _uiManager.RenderUI(_actionsWrapList);
    }


    private void ProcessingData(int column)
    {
        var shootCommand = new ShootCommand(squaresData, _actionsList, _actionsWrapList, column, nextSquareValue, boardRow);
        shootCommand.Excute();

        if (_actionsWrapList.Count <= 0)
        {
            _soundManager.PlaySoundMaxItemColumnSfx();
            return;
        }

        ProcessingLoop();
    }

    public void ProcessingLoop()
    {
        int countActionsList;

        var mergeCommand = new MergeCommand(squaresData, processingSquare, _actionsList, _actionsWrapList);
        var sortCommand = new SortCommand(squaresData, processingSquare, _actionsList, _actionsWrapList, boardCol);
        do
        {
            countActionsList = _actionsWrapList.Count;

            mergeCommand.Excute();
            sortCommand.Excute();
        } while (countActionsList < _actionsWrapList.Count);
    }

    private void CheckGameOver()
    {
        bool all = true;
        for (var index = 0; index < squaresData.Count; index++)
        {
            var squareData = squaresData[index];
            var isMaxItemColumnCanMerge = index >= 24 && Mathf.Approximately(squareData.value, nextSquareValue);
            if (squareData.value <= 0 || isMaxItemColumnCanMerge)
            {
                all = false;
                break;
            }
        }

        isGameOver = all;
    }

    private void OnApplicationQuit()
    {
        var saveGameCommand = new SaveGameCommand(_squareValueList, _isSave);
        saveGameCommand.Excute();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        CheckSaveGame(pauseStatus);
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        CheckSaveGame(!hasFocus);
    }

    private void CheckSaveGame(bool isPause)
    {
        if (isPause)
        {
            var saveGameCommand = new SaveGameCommand(_squareValueList, _isSave);
            saveGameCommand.Excute();
        }

        if (!isPause)
        {
            _isSave = false;
        }
    }

    private void LoadHighScore()
    {
        highScore = Prefs.HighScore;
    }
}