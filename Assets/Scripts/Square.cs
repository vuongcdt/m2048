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
            new (26, 188, 156, 225),
            new (46, 204, 113, 225),
            new (52, 152, 219, 225),
            new (155, 89, 182, 225),
            new (22, 160, 133, 225),
            new (39, 174, 96, 225),
            new (41, 128, 185, 225),
            new (241, 196, 15, 225),
            new (230, 126, 34, 225),
            new (231, 76, 60, 225),
        };
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

    public void ReturnPool()
    {
        // gameObject.SetActive(isActive);
        SharedGameObjectPool.Return(instantPool);
    }

    private void SetTextAndColor()
    {
        text.text = squareData.value == 0 ? "" : squareData.value.ToString();
        if (squareData.value == 0)
        {
            sprintRendererBg.color = _colors[0];
            return;
        }
        var exponent = Mathf.Log(squareData.value) / Mathf.Log(2);
        
        var colorIndex = (int)exponent % 10; 
        sprintRendererBg.color = _colors[colorIndex];
    }
}