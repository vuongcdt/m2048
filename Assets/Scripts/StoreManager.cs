// using System;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;
//
// [Serializable]
// public class StoreManager : Singleton<StoreManager>
// {
//     [SerializeField] private GameObject lineColumn;
//     [SerializeField] private Transform lineParentTransform;
//     [SerializeField] private List<SquareData> listSquareData = new();
//
//     private List<Square> _listSquare = new();
//     private List<GameObject> _listLineColumn = new();
//
//     public int rowTotal;
//     public int columnTotal;
//
//     public List<Square> ListSquare => _listSquare;
//
//     public void DeActiveLines()
//     {
//         _listLineColumn.ForEach(l => l.GetComponent<LineColumn>().SetActiveLine(false));
//     }
//
//     public SquareData GetSquareEmptyByColumn(int index)
//     {
//         // SetListSquareDataFromListSquare();
//
//         return listSquareData
//             .Where(s => s.cell.Column == index && s.value == 0)
//             .OrderBy(s => s.index)
//             .FirstOrDefault();
//     }
//
//     // public List<Square> GetSquaresNextToSameValue(Square processingSquare)
//     // {
//     //     SetListSquareDataFromListSquare();
//     //
//     //     var squaresNextToSameValue = _listSquare
//     //         .Where(s => IsEntryPassSquare(new SquareData(processingSquare), new SquareData(s)))
//     //         .ToList();
//     //
//     //     return squaresNextToSameValue;
//     // }
//     //
//     // private static bool IsEntryPassSquare(SquareData processingSquare, SquareData squareMap)
//     // {
//     //     return IsSameValue() && (IsNextToSameColumn(squareMap) || IsNexToSameRow(squareMap));
//     //
//     //     bool IsNexToSameRow(SquareData s) =>
//     //         (s.column == processingSquare.column + 1 || s.column == processingSquare.column - 1)
//     //         && s.row == processingSquare.row;
//     //
//     //     bool IsNextToSameColumn(SquareData s) =>
//     //         s.row == processingSquare.row - 1 && s.column == processingSquare.column;
//     //
//     //     bool IsSameValue() => processingSquare.value > 0 && squareMap.value == processingSquare.value;
//     // }
//     //
//     // public SquareData GetEmptySquarePrevRowByProcessingSquare(SquareData block)
//     // {
//     //     SetListSquareDataFromListSquare();
//     //
//     //     var emptySquare = listSquareData
//     //         .FirstOrDefault(squareData =>
//     //             squareData.value == 0 && block.column == squareData.column && block.row - 1 == squareData.row);
//     //     return emptySquare;
//     // }
//     //
//     // public void SetListSquareDataFromListSquare()
//     // {
//     //     _listSquare = _listSquare.Distinct().ToList();
//     //     foreach (var squareData in listSquareData)
//     //     {
//     //         var sd = _listSquare.FirstOrDefault(square =>
//     //             square.column == squareData.column && square.row == squareData.row && square.value > 0);
//     //         squareData.value = sd != null ? sd.value : 0;
//     //     }
//     //     //
//     //     // PrintListSquare();
//     //     // PrintListSquareData();
//     // }
//
//     private void Start()
//     {
//         RenderGrid();
//         RenderLineColumn();
//     }
//
//     private void RenderGrid()
//     {
//         for (var y = this.rowTotal; y > 0; y--)
//         {
//             for (var x = 0; x < this.columnTotal; x++)
//             {
//                 listSquareData.Add(new SquareData(this.rowTotal - y, x, x + (this.rowTotal - y) * this.columnTotal, 0));
//             }
//         }
//     }
//
//     private void RenderLineColumn()
//     {
//         for (int i = 0; i < this.columnTotal; i++)
//         {
//             var posLine = new Vector2(i * 2 - this.rowTotal, 0);
//             var line = Instantiate(lineColumn, posLine, Quaternion.identity, lineParentTransform);
//             line.GetComponent<LineColumn>().Column = i;
//             _listLineColumn.Add(line);
//         }
//     }
//
//     public void PrintListSquareData()
//     {
//         string str = "ListSquareData   ";
//         listSquareData
//             .Where(e => e.value > 0)
//             .ToList()
//             .ForEach(e => str += $"col: {e.column} row: {e.row} value: {e.value}");
//         print(str);
//     }
//
//     public void PrintListSquare()
//     {
//         string str = "+ListSquare    ";
//         _listSquare
//             .Where(e => e.value > 0)
//             .ToList()
//             .ForEach(e => str += $"col: {e.column} row: {e.row} value: {e.value}");
//         print(str);
//     }
// }