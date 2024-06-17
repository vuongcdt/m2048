using UnityEngine;

public class LineColumn : MonoCache
{
    [SerializeField] private SpriteRenderer bg;
    
    public int column;
    
    private void OnMouseDown()
    {
        SetActiveLine(true);
        boardManager.columnSelect = column;
        boardManager.isTouchLine = true;
    }

    private void OnMouseUp()
    {
        boardManager.isTouchLine = false;
        
        if (boardManager.isProcessing)
        {
            return;
        }
        StartCoroutine(boardManager.ShootBlock());
    }

    private void OnMouseEnter()
    {
        if (!boardManager.isTouchLine) return;
        boardManager.columnSelect = column;
        SetActiveLine(true);
    }

    private void OnMouseExit()
    {
        SetActiveLine(false);
    }
    
    private void SetActiveLine(bool value)
    {
        bg.color = value ? new Color(0.47f, 0.47f, 0.47f, 0.35f) : new Color(0.27f, 0.27f, 0.27f, 0.35f);
    }
}