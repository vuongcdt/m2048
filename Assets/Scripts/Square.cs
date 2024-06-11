using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Square : MonoBehaviour
{
    [SerializeField] private TextMesh text;
    [SerializeField] private int squareValue;
    [SerializeField] private int squareIndex;
    [SerializeField] private int column;
    [SerializeField] private int row;
    [SerializeField] private SpriteRenderer sprintRendererBg;
    [SerializeField] private Color squareColor;

    private List<Color> _colors;

    public Color SquareColor
    {
        get => squareColor;
        set => squareColor = value;
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

    public int SquareValue
    {
        get => squareValue;
        set => squareValue = value;
    }

    public int SquareIndex
    {
        get => squareIndex;
        set => squareIndex = value;
    }

    private void Awake()
    {
        _colors = new List<Color>(new[]
        {
            Constants.SquareColor.Blue,
            Constants.SquareColor.Red,
            Constants.SquareColor.Green,
            Constants.SquareColor.Green,
            Constants.SquareColor.Magenta
        });
    }

    private void Start()
    {
        SetTextAndColor();
    }

    private void OnEnable()
    {
        SetTextAndColor();
    }

    public void SetTextAndColor()
    {
        if (squareValue == 0)
        {
            return;
        }

        text.text = squareValue.ToString();
        var random = squareValue % 5 - 1;
        sprintRendererBg.color = _colors[random];
    }
}