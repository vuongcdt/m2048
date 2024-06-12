using System;
using System.Collections;
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
    private int _squareValue;
    private int _endValueSquareToPoint;
    private float _durationSquareToPoint;
    private bool _isRunToPoint;

    // public List<Square> ListSquare => _listSquare;
    public int SquareValue => _squareValue;
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
                var index = x + (row - y) * column;
                square.Value = 0;
                square.Index = index;
                square.Column = x;
                square.Row = row - y;

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
        if (_squareValue == 0)
        {
            SetRandomSquareValue();
        }
    }
    
    public List<Square> GetSquareEmpty(int index)
    {
        return _listSquare
            .Where(s => s.Column == index && s.Value == 0)
            .OrderBy(s => s.Index)
            .ToList();
    }
    
    public void SetSquare()
    {
        ProcessingSquare.Value = _squareValue;
        ProcessingSquare.SetTextAndColor();
        
        CompareSquare(ProcessingSquare);
        SetRandomSquareValue();
    }
    
    private void CompareSquare(Square processingSquare)
    {
        var squaresSame = _listSquare
            .Where(s => IsEntryPassSquare(processingSquare, s))
            .ToList();
        var squarePrevRow = squaresSame.FirstOrDefault(s => s.Row == processingSquare.Row - 1);

        var countSquare = squaresSame.Count;
        if (countSquare == 0)
        {
            return;
        }

        processingSquare.Value *= (int)Mathf.Pow(2, countSquare);
        squaresSame.ForEach(squareSameValue =>
        {
            squareSameValue.Value = 0;
            squareSameValue.SetTextAndColor();
        });

        processingSquare.SetTextAndColor();

        if (squarePrevRow is not null)
        {
            InvertedSquare(processingSquare, squarePrevRow);
        }
    }

    private static bool IsEntryPassSquare(Square squareCheck, Square square1)
    {
        bool IsNextToSameColumn(Square s) =>
            (s.Column == squareCheck.Column + 1 || s.Column == squareCheck.Column - 1)
            && s.Row == squareCheck.Row;

        bool IsNexToSameRow(Square s) => s.Row == squareCheck.Row - 1 && s.Column == squareCheck.Column;
        
        return square1.Value == squareCheck.Value && (IsNextToSameColumn(square1) || IsNexToSameRow(square1));
    }

    private void InvertedSquare(Square processingSquare, Square invertedSquare)
    {
        invertedSquare.Value = processingSquare.Value;
        invertedSquare.SetTextAndColor();

        processingSquare.Value = 0;
        processingSquare.SetTextAndColor();
        
        CompareSquare(invertedSquare);
    }


    private void SetRandomSquareValue()
    {
        _randomNum = Random.Range(0, _listSquareValue.Count);
        _squareValue = _listSquareValue[_randomNum];
        
        print("Value: " + _squareValue);//TODO
    }
}