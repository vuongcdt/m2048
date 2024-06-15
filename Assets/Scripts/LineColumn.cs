using UnityEngine;

public class LineColumn : MonoCache
{
    [SerializeField] private SpriteRenderer bg;
    
    private int _column;

    public int Column
    {
        get => _column;
        set => _column = value;
    }
    
    private void OnMouseDown()
    {
        SetActiveLine(true);
        boardManager.columnSelect = Column;
        boardManager.isTouchLine = true;
    }

    private void OnMouseUp()
    {
        boardManager.isTouchLine = false;
        boardManager.ShootBlock(_column);
    }

    private void SetActiveLine(bool value)
    {
        bg.color = value ? new Color(0.47f, 0.47f, 0.47f, 0.35f) : new Color(0.27f, 0.27f, 0.27f, 0.35f);
    }

    private void OnMouseEnter()
    {
        if (!boardManager.isTouchLine) return;
        boardManager.columnSelect = Column;
        SetActiveLine(true);
    }

    private void OnMouseExit()
    {
        SetActiveLine(false);
    }
}