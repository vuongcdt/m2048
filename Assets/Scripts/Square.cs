using System.Collections.Generic;
using UnityEngine;

public class Square : MonoCache
{
    [SerializeField] private TextMesh text;
    [SerializeField] private SpriteRenderer sprintRendererBg;
    
    private List<Color> _colors;

    public SquareData squareData;

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

    private void SetTextAndColor()
    {
        text.text = squareData.value == 0 ? "" : squareData.value.ToString();
        var random = squareData.value % 7; //TODO
        sprintRendererBg.color = _colors[random];
    }
}