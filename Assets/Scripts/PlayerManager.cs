using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    [SerializeField] private Camera cameraMain;
    [SerializeField] private GameObject lineColumn;
    [SerializeField] private Transform parentTransform;

    private BoardManager _boardManager;
    private int _columnIndex;

    private int _endValueSquareToPoint;
    private float _durationSquareToPoint;
    private bool _isRunToPoint;
    private readonly List<GameObject> _listLineColumn = new();

    public int EndValueSquareToPoint => _endValueSquareToPoint;

    public float DurationSquareToPoint => _durationSquareToPoint;

    private void Start()
    {
        _boardManager = BoardManager.Instance;
        RenderLineColumn();
    }

    private void RenderLineColumn()
    {
        for (int i = 0; i < _boardManager.Column; i++)
        {
            var posLine = new Vector2(i * 2 - _boardManager.Row, 0);
            lineColumn.SetActive(false);
            lineColumn.GetComponent<LineColumn>().Column = i;
            _listLineColumn.Add(Instantiate(lineColumn, posLine, Quaternion.identity, parentTransform));
        }
    }

    private void Update()
    {
        CheckPointClick();
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

        if (!Input.GetMouseButtonUp(0))
        {
            return;
        }

        var squareEmpty = _boardManager.GetSquareEmpty(_columnIndex);

        if (squareEmpty.Count == 0)
        {
            return;
        }

        StartCoroutine(DeActiveLineColumn());
    }

    private void ShowLineColumn()
    {
        var worldPos = cameraMain.ScreenToWorldPoint(Input.mousePosition);

        var index = (int)(worldPos.x / 2 + 3);
        if (index < 0 || index > _boardManager.Column - 1)
        {
            return;
        }

        var squareEmpty = _boardManager.GetSquareEmpty(index);

        if (squareEmpty.Count == 0)
        {
            return;
        }

        _isRunToPoint = true;
        _endValueSquareToPoint = (squareEmpty.Count - 3) * 2;
        _boardManager.ProcessingSquare = squareEmpty.FirstOrDefault();
        _durationSquareToPoint = Constants.TimeMove.TimeSquareMoveToPoint * squareEmpty.Count / 5;

        _columnIndex = index;
        _listLineColumn.ForEach(lineObj => lineObj.SetActive(false));
        _listLineColumn[_columnIndex].SetActive(true);
    }

    private IEnumerator DeActiveLineColumn()
    {
        yield return new WaitForSeconds(_durationSquareToPoint);

        SetSquareToBoard();
    }

    private void SetSquareToBoard()
    {
        _listLineColumn[_columnIndex].SetActive(false);

        if (!_boardManager.ProcessingSquare)
        {
            return;
        }

        _boardManager.SetSquare();
        _isRunToPoint = false;
    }
}