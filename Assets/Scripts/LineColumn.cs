using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class LineColumn : MonoBehaviour
{
    [SerializeField] private GameObject square;

    private Square _squareScript;
    private BoardManager _boardManager;
    private PlayerManager _playerManager ;

    private int _column;

    public int Column
    {
        get => _column;
        set => _column = value;
    }

    private void Awake()
    {
        _squareScript = square.GetComponent<Square>();
        _boardManager = BoardManager.Instance;
        _playerManager = PlayerManager.Instance;
    }

    private void OnEnable()
    {
        SquareMoveToPoint();
    }

    private void SquareMoveToPoint()
    {
        _squareScript.Value = _boardManager.SquareValue;

        square.transform
            .DOMoveY(_playerManager.EndValueSquareToPoint, _playerManager.DurationSquareToPoint)
            .SetEase(Ease.Linear)
            // .OnComplete(() => { Debug.Log("square.transform.position: " + square.transform.position); })
            ;
    }

    private void OnDisable()
    {
        square.transform.position = new Vector2(square.transform.position.x, -5);
        _squareScript.Color = Constants.SquareColor.White;
        _squareScript.Value = 0;
    }
}