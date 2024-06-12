using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : Singleton<BoardManager>
{
    [SerializeField] private Square square;
    [SerializeField] private Transform parentTransform;
    [SerializeField] private int row;
    [SerializeField] private int column;

    private readonly List<Square> _listSquare = new();
    private readonly List<int> _listSquareValue = new() { 2, 4 };
    // private readonly List<int> _listSquareValue = new() { 2, 4, 8, 16, 32, 64, 128 };

    private int _columnIndex;
    private int _randomNum;
    private int _squareNextValue;
    private int _endValueSquareToPoint;
    private float _durationSquareToPoint;
    private bool _isRunToPoint;

    // public List<Square> ListSquare => _listSquare;
    public int SquareNextValue => _squareNextValue;
    public int Row => row;
    public int Column => column;

    public Square ProcessingSquare { get; set; }

    private void Start()
    {
        RenderSquareBoard();
    }

    private void RenderSquareBoard()
    {
        for (var y = row; y > 0; y--)
        {
            for (var x = 0; x < column; x++)
            {
                square.Value = 0;
                square.Column = x;
                square.Row = row - y;
                square.Index = x + (row - y) * column;

                _listSquare.Add(Instantiate(square,
                    new Vector2(x * 2 - row, y * 2 - column),
                    Quaternion.identity,
                    parentTransform
                ));
            }
        }
    }

    private void FixedUpdate()
    {
        if (_squareNextValue == 0)
        {
            SetRandomSquareValue();
        }
    }

    public List<Square> GetSquareEmptyByColumn(int index)
    {
        return _listSquare
            .Where(s => s.Column == index && s.Value == 0)
            .OrderBy(s => s.Index)
            .ToList();
    }

    public void SetSquare()
    {
        ProcessingSquare.Value = _squareNextValue;

        CompareSquare(ProcessingSquare);
        SetRandomSquareValue();
    }

    private void CompareSquare(Square processingSquare)
    {
        var squaresSame = _listSquare
            .Where(s => IsEntryPassSquare(processingSquare, s))
            .ToList();

        var countSquare = squaresSame.Count;
        if (countSquare == 0)
        {
            return;
        }

        processingSquare.Value *= (int)Mathf.Pow(2, countSquare);

        squaresSame.ForEach(squareSameValue => squareSameValue.Value = 0);

        FillBoard();
    }

    private void FillBoard()
    {
        Square SquareHasValueSameColumnNextRow(Square s) => _listSquare
            .FirstOrDefault(e => e.Column == s.Column && e.Value != 0 && e.Row == s.Row + 1);

        var emptySquares = _listSquare
            .Where(s => s.Value == 0 && SquareHasValueSameColumnNextRow(s))
            .ToList();

        emptySquares.ForEach(squareEmpty => InvertedSquare(SquareHasValueSameColumnNextRow(squareEmpty), squareEmpty));
    }

    private static bool IsEntryPassSquare(Square squareCheck, Square square1)
    {
        bool IsNextToSameColumn(Square s) =>
            (s.Column == squareCheck.Column + 1 || s.Column == squareCheck.Column - 1)
            && s.Row == squareCheck.Row;

        bool IsNexToSameRow(Square s) => s.Row == squareCheck.Row - 1 && s.Column == squareCheck.Column;

        return square1.Value == squareCheck.Value && (IsNextToSameColumn(square1) || IsNexToSameRow(square1));
    }

    private void InvertedSquare(Square squareHasValue, Square squareEmpty)
    {
        squareEmpty.Value = squareHasValue.Value;
        squareHasValue.Value = 0;

        CompareSquare(squareEmpty);
    }


    private void SetRandomSquareValue()
    {
        _randomNum = Random.Range(0, _listSquareValue.Count);
        _squareNextValue = _listSquareValue[_randomNum];
    }
}