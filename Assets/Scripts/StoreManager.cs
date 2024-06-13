using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class StoreManager : Singleton<StoreManager>
{
    [SerializeField] private GameObject lineColumn;
    [SerializeField] private Transform lineParentTransform;
    [SerializeField] private List<SquareData> listSquareData = new();

    private readonly List<Square> _listSquare = new();
    private readonly List<GameObject> _listLineColumn = new();

    public int rowTotal;
    public int columnTotal;

    public List<SquareData> ListSquareData
    {
        get => listSquareData;
        set => listSquareData = value;
    }

    public List<Square> ListSquare => _listSquare;

    public void DeActiveLines()
    {
        _listLineColumn.ForEach(l => l.GetComponent<LineColumn>().SetActiveLine(false));
    }

    public SquareData GetSquareEmptyByColumn(int index)
    {
        return listSquareData
            .Where(s => s.column == index && s.value == 0)
            .OrderBy(s => s.index)
            .FirstOrDefault();
    }

    public List<SquareData> GetSquaresDataNextToSameValue(SquareData processingSquareData)
    {
        return listSquareData
            .Where(s => IsEntryPassSquare(processingSquareData, s))
            .ToList();
    }

    public List<Square> GetSquaresNextToSameValue(SquareData processingSquareData)
    {
        var squaresNextToSameValue = _listSquare
            .Where(s => IsEntryPassSquare(processingSquareData, new SquareData(s)))
            .ToList();

        return squaresNextToSameValue;
    }

    private static bool IsEntryPassSquare(SquareData squareCheck, SquareData squareMap)
    {
        return IsSameValue() && (IsNextToSameColumn(squareMap) || IsNexToSameRow(squareMap));

        bool IsNexToSameRow(SquareData s) =>
            (s.column == squareCheck.column + 1 || s.column == squareCheck.column - 1)
            && s.row == squareCheck.row;

        bool IsNextToSameColumn(SquareData s) => s.row == squareCheck.row - 1 && s.column == squareCheck.column;

        bool IsSameValue() => squareCheck.value > 0 && squareMap.value == squareCheck.value;
    }

    public SquareData GetEmptySquarePrevRow(SquareData squareData)
    {
        var emptySquare = listSquareData
            .FirstOrDefault(block =>
                squareData.column == block.column && block.value == 0 && squareData.row == block.row + 1);

        return emptySquare;
    }

    public void SetListSquareDataBylistSquare(bool isPrint = false)
    {
        if(isPrint)PrintListSquare();

        listSquareData
            .ForEach(squareData =>
            {
                var sData = _listSquare.FirstOrDefault(square =>
                    square.column == squareData.column && square.row == squareData.row);

                if (sData is not null)
                {
                    if(isPrint)print($"col {sData.column} row: {sData.row} Value: {sData.value}  index:{sData.index}");
                    squareData.value = sData.value;
                }
            });

        if(isPrint)PrintListSquareData();
    }

    private void PrintListSquareData()
    {
        string str = "ListSquareData   ";
        listSquareData
            .Where(e => e.value > 0)
            .ToList()
            .ForEach(e => str += $"col: {e.column} row: {e.row} value: {e.value} index: {e.index}  ");
        print(str);
    }

    private void PrintListSquare()
    {
        string str = "+ListSquare    ";
        _listSquare
            .Where(e => e.value > 0)
            .ToList()
            .ForEach(e => str += $"col: {e.column} row: {e.row} value: {e.value} index: {e.index}  ");
        print(str);
    }

    private void Start()
    {
        RenderGrid();
        RenderLineColumn();
    }

    private void RenderGrid()
    {
        for (var y = this.rowTotal; y > 0; y--)
        {
            for (var x = 0; x < this.columnTotal; x++)
            {
                listSquareData.Add(new SquareData(this.rowTotal - y, x, x + (this.rowTotal - y) * this.columnTotal, 0));
            }
        }
    }

    private void RenderLineColumn()
    {
        for (int i = 0; i < this.columnTotal; i++)
        {
            var posLine = new Vector2(i * 2 - this.rowTotal, 0);
            var line = Instantiate(lineColumn, posLine, Quaternion.identity, lineParentTransform);
            line.GetComponent<LineColumn>().Column = i;
            _listLineColumn.Add(line);
        }
    }
}