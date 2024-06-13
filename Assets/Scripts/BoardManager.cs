using System.Collections;
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

    private readonly List<int> _listSquareValue = new() { 2 };
    // private readonly List<int> _listSquareValue = new() { 2, 4, 8, 16, 32, 64, 128 };

    private int _randomNum;
    private int _squareNextValue;
    private bool _isCheckSquare;
    private StoreManager _storeManager;

    public bool isTouchLine;
    public int columnSelect;
    public int SquareNextValue => _squareNextValue;
    public SquareData processingSquareData;
    public Square processingSquare;

    private void Start()
    {
        _storeManager = StoreManager.Instance;
        SetRandomSquareValue();
    }

    private void SetRandomSquareValue()
    {
        _randomNum = Random.Range(0, _listSquareValue.Count);
        _squareNextValue = _listSquareValue[_randomNum];
    }

    public void ShootBlock()
    {
        var emptySquare = _storeManager.GetSquareEmptyByColumn(columnSelect);
        var targetPos = Utils.GridToPos(emptySquare.row, emptySquare.column);
        var newBlock = Instantiate(square, Utils.GridToPos(6, columnSelect), Quaternion.identity, parentTransform);

        newBlock.value = _squareNextValue;

        processingSquareData = emptySquare;
        processingSquare = newBlock;

        newBlock.MoveY(targetPos.y, MergeSquare);
    }

    private void MergeSquare()
    {
        processingSquare.Init(_squareNextValue);
        _storeManager.ListSquare.Add(processingSquare);
        processingSquareData.value = _squareNextValue;

        var squaresSame = _storeManager.GetSquaresNextToSameValue(processingSquareData);
        // var squaresDataSame = _storeManager.GetSquaresDataNextToSameValue(processingSquareData);
        var countSquare = squaresSame.Count;

        if (countSquare == 0)
        {
            SetRandomSquareValue();
            _isCheckSquare = false;
            return;
        }

        squaresSame.ForEach(block =>
        {
            block.MoveToPos(processingSquareData.Position, () => block.value = 0);
        });

        StartCoroutine(SetTime(countSquare));
    }

    private IEnumerator SetTime(float countSquare)
    {
        yield return new WaitForSeconds(0.3f);
        // squaresDataSame.ForEach(squareData => squareData.value = 0);

        processingSquare.value *= (int)Mathf.Pow(2, countSquare);
        processingSquare.Init(processingSquare.value);
        _storeManager.SetListSquareDataBylistSquare(true);
        // processingSquareData.value *= (int)Mathf.Pow(2, countSquare);

        FillBoard();
        SetRandomSquareValue();
    }

    private void FillBoard()
    {
        var emptySquares = _storeManager.GetEmptySquarePrevRow(processingSquareData);

        print($"emptySquares: {emptySquares}");
        if (emptySquares is null)
        {
            return;
        }

        processingSquare.MoveY(emptySquares.Position.y, () =>
        {
            // emptySquares.value = processingSquareData.value;
            // processingSquareData.value = 0;
            
            processingSquare.SetCell();
            _storeManager.SetListSquareDataBylistSquare();
            // processingSquareData = emptySquares;
        });
    }
}