using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Square : MonoCache
{
    [SerializeField] private TextMesh text;
    [SerializeField] private SpriteRenderer sprintRendererBg;
    
    private List<Color> _colors;

    public SquareData squareData;
    private const float TIME_MERGE_SQUARE = 0.1f;

    private void Awake()
    {
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

    public void SetIndex(int index)
    {
        squareData.index = index;
    }
    public void SetValue( int newValue)
    {
        squareData.value = newValue;
        SetTextAndColor();
    }
    private void SetTextAndColor()
    {
        text.text = squareData.value == 0 ? "" : squareData.value.ToString();
        var random = squareData.value % 7; //TODO
        sprintRendererBg.color = _colors[random];
    }

}