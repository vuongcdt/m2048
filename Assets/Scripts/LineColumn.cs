using UnityEngine;

public class LineColumn : MonoCache
{
    [SerializeField] private SpriteRenderer bg;
    
    public int column;
    private bool _isPlaying;
    
    private void OnMouseDown()
    {
        if(!boardManager.isPlaying)
        {
            return;
        }
        SetActiveLine(true);
        boardManager.columnSelect = column;
        boardManager.isTouchLine = true;
    }

    private void OnMouseUp()
    {
        if(!boardManager.isPlaying)
        {
            return;
        }
        boardManager.isTouchLine = false;
        
        if (boardManager.isProcessing)
        {
            return;
        }
        StartCoroutine(boardManager.ShootBlock());
        SetActiveLine(false);
    }

    private void OnMouseEnter()
    {
        if(!boardManager.isPlaying)
        {
            return;
        }
        if (!boardManager.isTouchLine)
        {
            return;
        }
        boardManager.columnSelect = column;
        SetActiveLine(true);
    }

    private void OnMouseExit()
    {
        if(!boardManager.isPlaying)
        {
            return;
        }
        SetActiveLine(false);
    }
    
    private void SetActiveLine(bool value)
    {
        bg.color = value ? new Color(0.47f, 0.47f, 0.47f, 0.35f) : new Color(0.27f, 0.27f, 0.27f, 0.35f);
    }
}