using System.Collections.Generic;
using DG.Tweening;
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

    public void SetId(int id)
    {
        squareData.id = id;
    }

    public void SetValue(int newValue)
    {
        squareData.value = newValue;
        SetTextAndColor();
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    private void SetTextAndColor()
    {
        text.text = squareData.value == 0 ? "" : squareData.value.ToString();
        var random = squareData.value % 7; //TODO
        sprintRendererBg.color = _colors[random];
    }
}