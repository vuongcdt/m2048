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
    private StoreManager _storeManager;
    private bool _isMergeSquare;


    public bool isTouchLine;
    public int columnSelect;
    public int SquareNextValue => _squareNextValue;
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

        _isMergeSquare = true;
        processingSquare = newBlock;
        processingSquare.MoveY(targetPos.y, MergeSquare);
    }

    private void MergeSquare()
    {
        _storeManager.ListSquare.Add(processingSquare);
        var squaresSame = _storeManager.GetSquaresNextToSameValue(processingSquare);
        var countSquare = squaresSame.Count;

        // Debug.Log($"countSquare: {countSquare}");
        if (countSquare == 0)
        {
            _isMergeSquare = false;
            SetRandomSquareValue();
            return;
        }

        processingSquare.value *= (int)Mathf.Pow(2, countSquare);

        squaresSame.ForEach(block =>
        {
            block.MoveToPos(
                processingSquare.transform.position,
                () =>
                {
                    block.value = 0;
                    FillBoard();
                    SetRandomSquareValue();
                });
        });
    }

    private void FillBoard()
    {
        var emptySquares = _storeManager.GetEmptySquarePrevRow(new SquareData(processingSquare));

        if (emptySquares is null)
        {
            // Debug.Log("emptySquares null roi");
            MergeSquare();
            return;
        }

        processingSquare.MoveToPos(emptySquares.Position, MergeSquare, false);
    }
}