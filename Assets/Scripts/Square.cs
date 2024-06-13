using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Square : MonoBehaviour
{
    [SerializeField] private TextMesh text;
    [SerializeField] private SpriteRenderer sprintRendererBg;

    public int value;
    public int index;
    public int column;
    public int row;
    public Color color;

    private List<Color> _colors;
    private const float TIME_MERGE_SQUARE = 0.2f;
    private StoreManager _storeManager;

    public void Init(int numValue)
    {
        var cell = Utils.PosToGrid(transform.position);
        this.row = cell.Row;
        this.column = cell.Column;
        this.index = cell.Row + (_storeManager.rowTotal - cell.Column) * _storeManager.columnTotal;
        this.value = numValue;

        SetTextAndColor();
    }
    
    public void SetCell()
    {
        var cell = Utils.PosToGrid(transform.position);
        this.row = cell.Row;
        this.column = cell.Column;
        this.index = cell.Row + (_storeManager.rowTotal - cell.Column) * _storeManager.columnTotal;
    }

    public void MoveToPos(Vector2 pos,TweenCallback onComplete = null)
    {
        transform.DOMove(pos, TIME_MERGE_SQUARE)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
                onComplete?.Invoke();
            });
    }

    public void MoveY(float posY, TweenCallback onComplete = null)
    {
        var duration = (posY / 2 + 3) / 10;

        this.transform
            .DOMoveY(posY, duration)
            .SetEase(Ease.Linear)
            .OnComplete(onComplete);
    }

    public void MergeSquareX(float posX, TweenCallback onComplete = null)
    {
        this.transform
            .DOMoveX(posX, TIME_MERGE_SQUARE)
            .SetEase(Ease.Linear)
            .OnComplete(onComplete);
    }

    public void MergeSquareY(float posY, TweenCallback onComplete = null)
    {
        this.transform
            .DOMoveY(posY, TIME_MERGE_SQUARE)
            .SetEase(Ease.Linear)
            .OnComplete(onComplete);
    }

    private void Awake()
    {
        _storeManager = StoreManager.Instance;
        _colors = new List<Color>(new[]
        {
            Constants.SquareColor.White,
            Constants.SquareColor.Red,
            Constants.SquareColor.Blue,
            Constants.SquareColor.Green,
            Constants.SquareColor.Yellow,
            Constants.SquareColor.Magenta,
            Constants.SquareColor.Cyan,
            Constants.SquareColor.Grey,
        });
        SetTextAndColor();
    }

    private void FixedUpdate()
    {
        SetTextAndColor();
    }

    private void SetTextAndColor()
    {
        text.text = value == 0 ? "" : value.ToString();
        var random = value % 7; //TODO
        sprintRendererBg.color = _colors[random];
    }
}