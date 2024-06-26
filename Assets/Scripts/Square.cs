using System.Collections.Generic;
using UnityEngine;

public class Square : MonoCache
{
    [SerializeField] private TextMesh text;
    [SerializeField] private SpriteRenderer sprintRendererBg;

    public SquareData squareData;

    protected override void AwakeCustom()
    {
        SetTextAndColor();
    }

    public void SetId(int id)
    {
        squareData.id = id;
    }

    public void SetValue(float newValue)
    {
        squareData.value = newValue;
        SetTextAndColor();
    }

    public void SetActiveObj(bool isActive)
    {
        gameObjectCache.SetActive(isActive);
    }

    public void ReturnPool()
    {
        UIManager.Instance.ReturnPool(gameObjectCache);
    }

    private void SetTextAndColor()
    {
        var valueFormat = Utils.GetText(squareData.value);

        text.text = squareData.value == 0 ? "" : valueFormat;
        if (squareData.value == 0)
        {
            return;
        }
        sprintRendererBg.color = Utils.GetColor(squareData.value);
    }
}