using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : Singleton<BoardManager>
{
    [SerializeField] private Square square;
    [SerializeField] private GameObject lineColumn;
    [SerializeField] private Transform parentTransform;
    [SerializeField] private Camera cameraMain;
    [SerializeField] private int row;
    [SerializeField] private int column;

    private readonly List<Square> _listSquare = new();
    private readonly List<GameObject> _listLineColumn = new();
    private readonly List<int> _listSquareValue = new() { 2, 4, 8, 16, 32, 64, 128 };

    private int _columnIndex;
    private int _randomNum;
    private int _squareValue;
    private int _endValueSquareToPoint;
    private float _durationSquareToPoint;
    private bool _isRunToPoint;
    private Square _squareOnBoard;

    public List<Square> ListSquare => _listSquare;
    public int EndValueSquareToPoint => _endValueSquareToPoint;
    public float DurationSquareToPoint => _durationSquareToPoint;

    public int SquareValue
    {
        get => _squareValue;
        set => _squareValue = value;
    }

    void Start()
    {
        RenderSquareBoard();
        RenderLineColumn();
    }

    private void RenderLineColumn()
    {
        for (int i = 0; i < column; i++)
        {
            var posLine = new Vector2(i * 2 - row, 0);
            lineColumn.SetActive(false);
            lineColumn.GetComponent<LineColumn>().Column = i;
            _listLineColumn.Add(Instantiate(lineColumn, posLine, Quaternion.identity, parentTransform));
        }
    }

    private void RenderSquareBoard()
    {
        for (var y = row; y > 0; y--)
        {
            for (var x = 0; x < column; x++)
            {
                var index = x + (row - y) * column;
                square.SquareValue = 0;
                square.SquareIndex = index;
                square.Column = x;
                square.Row = y;

                _listSquare.Add(Instantiate(square,
                    new Vector2(x * 2 - row, y * 2 - column),
                    Quaternion.identity,
                    parentTransform
                ));
            }
        }
    }

    private void Update()
    {
        CheckPointClick();

        if (_squareValue == 0)
        {
            SetRandomSquareValue();
        }
    }

    private void CheckPointClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_isRunToPoint)
            {
                return;
            }

            ShowLineColumn();
        }

        if (Input.GetMouseButtonUp(0))
        {
            var squareEmpty = GetSquareEmpty(_columnIndex);
        
            if (squareEmpty.Count == 0 )
            {
                return;
            }
            StartCoroutine(DeActiveLineColumn());
        }
    }

    private void ShowLineColumn()
    {
        
        var worldPos = cameraMain.ScreenToWorldPoint(Input.mousePosition);

        var index = (int)(worldPos.x / 2 + 3);
        if (index < 0 || index > column - 1)
        {
            return;
        }

        var squareEmpty = GetSquareEmpty(index);
        
        if (squareEmpty.Count == 0)
        {
            return;
        }
        _isRunToPoint = true;
        _endValueSquareToPoint = (squareEmpty.Count - 3) * 2;
        _squareOnBoard = squareEmpty.FirstOrDefault();
        _durationSquareToPoint = Constants.TimeMove.TimeSquareMoveToPoint * squareEmpty.Count / 5;

        _columnIndex = index;
        _listLineColumn.ForEach(lineObj => lineObj.SetActive(false));
        _listLineColumn[_columnIndex].SetActive(true);
    }

    private List<Square> GetSquareEmpty(int index)
    {
        return _listSquare
            .Where(s => s.Column == index && s.SquareValue == 0)
            .OrderBy(s => s.SquareIndex)
            .ToList();
    }

    private IEnumerator DeActiveLineColumn()
    {
        yield return new WaitForSeconds(_durationSquareToPoint);
     
        SetSquareToBoard();
    }

    private void SetSquareToBoard()
    {
        _listLineColumn[_columnIndex].SetActive(false);

        if (!_squareOnBoard)
        {
            return;
        }

        _squareOnBoard.SquareValue = _squareValue;
        _squareOnBoard.SetTextAndColor();
        CompareSquare();
        SetRandomSquareValue();
        _isRunToPoint = false;
    }

    private void CompareSquare()
    {
        throw new NotImplementedException();
    }

    private void SetRandomSquareValue()
    {
        _randomNum = Random.Range(0, column + 1);
        _squareValue = _listSquareValue[_randomNum];
        print("Value: " + _squareValue);
    }
}