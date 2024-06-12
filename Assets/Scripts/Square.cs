using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Square : MonoBehaviour
{
    [SerializeField] private TextMesh text;
    [SerializeField] private int value;
    [SerializeField] private int index;
    [SerializeField] private int column;
    [SerializeField] private int row;
    [SerializeField] private SpriteRenderer sprintRendererBg;
    [SerializeField] private Color color;

    private List<Color> _colors;

    public Color Color
    {
        get => color;
        set => color = value;
    }

    public int Row
    {
        get => row;
        set => row = value;
    }

    public int Column
    {
        get => column;
        set => column = value;
    }

    public int Value
    {
        get => value;
        set => this.value = value;
    }

    public int Index
    {
        get => index;
        set => index = value;
    }

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

    private void FixedUpdate()
    {
        SetTextAndColor();
    }

    private void SetTextAndColor()
    {
        text.text = value == 0 ? "" : value.ToString();
        var random = value % 7;//TODO
        sprintRendererBg.color = _colors[random];
    }
}