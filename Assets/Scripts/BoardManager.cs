using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class BoardManager : Singleton<BoardManager>
{
    [SerializeField] private Square square;
    [SerializeField] private Transform parentTransform;
    [SerializeField] private int row;
    [SerializeField] private int column;

    [SerializeField] List<SquareData> _listSquareData = new();

    private readonly List<int> _listSquareValue = new() { 2, 4 };
    // private readonly List<int> _listSquareValue = new() { 2, 4, 8, 16, 32, 64, 128 };

    private int _randomNum;
    private int _squareNextValue;
    private bool _isCheckSquare;
    public bool isTouchLine;
    public int columnSelect;

    public int SquareNextValue => _squareNextValue;
    public int Row => row;
    public int Column => column;

    public SquareData ProcessingSquare { get; set; }

    private void Start()
    {
        for (var y = row; y > 0; y--)
        {
            for (var x = 0; x < column; x++)
            {
                _listSquareData.Add(new SquareData(row - y, x, x + (row - y) * column, 0));
            }
        }

        SetRandomSquareValue();
    }

    public List<SquareData> GetSquareEmptyByColumn(int index)
    {
        return _listSquareData
            .Where(s => s.column == index && s.value == 0)
            .OrderBy(s => s.index)
            .ToList();
    }

    public void SetSquare()
    {
        ProcessingSquare.value = _squareNextValue;

        _isCheckSquare = true;
        CompareSquare(ProcessingSquare);
        _isCheckSquare = false;

        SetRandomSquareValue();
    }

    private void CompareSquare(SquareData processingSquare)
    {
        var squaresSame = _listSquareData
            .Where(s => IsEntryPassSquare(processingSquare, s))
            .ToList();

        var countSquare = squaresSame.Count;

        if (countSquare == 0)
        {
            _isCheckSquare = false;
            return;
        }

        processingSquare.value *= (int)Mathf.Pow(2, countSquare);
        squaresSame.ForEach(squareSameValue => squareSameValue.value = 0);

        FillBoard();

        if (_isCheckSquare)
        {
            CompareSquare(ProcessingSquare);
        }
    }

    private void FillBoard()
    {
        var emptySquares = _listSquareData
            .Where(s => s.value == 0 && SquareHasValueSameColumnNextRow(s) != null)
            .ToList();

        if (emptySquares.Count == 0)
        {
            return;
        }

        emptySquares.ForEach(squareEmpty => InvertedSquare(SquareHasValueSameColumnNextRow(squareEmpty), squareEmpty));

        return;

        SquareData SquareHasValueSameColumnNextRow(SquareData s) => _listSquareData
            .FirstOrDefault(e => e.column == s.column && e.value != 0 && e.row == s.row + 1);
    }

    private static bool IsEntryPassSquare(SquareData squareCheck, SquareData squareMap)
    {
        return IsSameValue() && (IsNextToSameColumn(squareMap) || IsNexToSameRow(squareMap));

        bool IsNextToSameColumn(SquareData s) =>
            (s.column == squareCheck.column + 1 || s.column == squareCheck.column - 1)
            && s.row == squareCheck.row;

        bool IsNexToSameRow(SquareData s) => s.row == squareCheck.row - 1 && s.column == squareCheck.column;

        bool IsSameValue() => squareCheck.value > 0 && squareMap.value == squareCheck.value;
    }

    private void InvertedSquare(SquareData squareHasValue, SquareData squareEmpty)
    {
        squareEmpty.value = squareHasValue.value;
        squareHasValue.value = 0;

        CompareSquare(squareEmpty);
    }


    private void SetRandomSquareValue()
    {
        _randomNum = Random.Range(0, _listSquareValue.Count);
        _squareNextValue = _listSquareValue[_randomNum];
    }

    public void ShootBlock()
    {
        var emptySquare = GetSquareEmptyByColumn(columnSelect).First();
        var targetPos = GridToPos(emptySquare.row, emptySquare.column);
        var newBlock = Instantiate(square, GridToPos(6, columnSelect), Quaternion.identity, parentTransform);

        newBlock.Value = _squareNextValue;
        var duration = (targetPos.y / 2 + 3) / 10;

        newBlock.transform
            .DOMoveY(targetPos.y, duration)
            .SetEase(Ease.Linear)
            .OnComplete(() => { MergeSquare(emptySquare); });
    }

    private void MergeSquare(SquareData emptySquare)
    {
        emptySquare.value = _squareNextValue;
        SetRandomSquareValue();
    }

    private Vector2 GridToPos(int row, int col)
    {
        return new Vector2(-5 + col * 2, 4 - row * 2);
    }

    private Cell PosToGrid(Vector2 pos)
    {
        return new Cell((int)(pos.y + 10) / 2, (int)(8 - pos.x) / 2);
    }

    private class Cell
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public Cell(int row, int column)
        {
            Row = row;
            Column = column;
        }
    }
}