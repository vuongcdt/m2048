using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using uPools;

public class Square : MonoCache
{
    [SerializeField] private TextMesh text;
    [SerializeField] private SpriteRenderer sprintRendererBg;
    public GameObject instantPool;
    private List<Color32> _colors;

    public SquareData squareData;

    private void Awake()
    {
        _colors = new List<Color32>
        {
            // new (255, 255, 255,255), 
            
            new (22, 160, 133,255),
            new (41, 128, 185,255),
            new (142, 68, 173,255),
            new (243, 156, 18,255),
            new (211, 84, 0,255),
            new (192, 57, 43,255),
            new (39, 174, 96,255),
            new (232, 67, 147,255),
            new (116, 185, 255,255),
            new (162, 155, 254,255),

            // new (210, 245, 60,255),
            // new (70, 240, 240,255),
            // new (0, 130, 200,255),
            // new (60, 180, 75,255), 
            // new (162, 155, 254,255),
            // new (255, 225, 25,255),
            // new (245, 130, 48,255),
            // new (230, 25, 75,255), 
            // new (145, 30, 180,255),
            // new (240, 50, 230,255),
        };
        SetTextAndColor();
    }

    public void SetId(int id)
    {
        squareData.id = id;
    }

    public void SetValue(long newValue)
    {
        squareData.value = newValue;
        SetTextAndColor();
    }

    public void ReturnPool()
    {
        // gameObject.SetActive(isActive);
        SharedGameObjectPool.Return(instantPool);
    }

    private void SetTextAndColor()
    {
        var valueFormat = squareData.value.ToString();
        switch (squareData.value)
        {
            case >= 1024 * 1024 * 1024:
                valueFormat = squareData.value / (1024 * 1024 * 1024)  + "B";
                break;
            case >= 1024 * 1024:
                valueFormat = squareData.value / (1024 * 1024)  + "M";
                break;
            case >= 1024 * 16:
                valueFormat = squareData.value / 1024  + "K";
                break;
        }
        text.text = squareData.value == 0 ? "" : valueFormat;
        if (squareData.value == 0)
        {
            // sprintRendererBg.color = _colors[0];
            return;
        }

        var exponent = Utils.GetExponent(squareData.value);

        var colorIndex = exponent % 10;
        sprintRendererBg.color = _colors[colorIndex];
    }
}