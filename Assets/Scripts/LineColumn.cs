using UnityEngine;

public class LineColumn : MonoCache
{
    [SerializeField] private GameObject square;
    [SerializeField] private SpriteRenderer bg;

    private Square _squareScript;
    // private BoardManager _boardManager;
    // private StoreManager _storeManager;

    private int _column;

    public int Column
    {
        get => _column;
        set => _column = value;
    }

    private void Awake()
    {
        boardManager = BoardManager.Instance;
        // _storeManager = StoreManager.Instance;
    }

    private void OnMouseDown()
    {
        SetActiveLine(true);
        // boardManager.columnSelect = Column;
        // boardManager.isTouchLine = true;
    }

    private void OnMouseUp()
    {
        // _storeManager.DeActiveLines();
        // boardManager.isTouchLine = false;
        // boardManager.ShootBlock();
    }

    public void SetActiveLine(bool value)
    {
        bg.color = value ? Color.red : new Color(0.27f, 0.27f, 0.27f, 0.35f);
    }

    private void OnMouseEnter()
    {
        // if (!boardManager.isTouchLine) return;
        // boardManager.columnSelect = Column;
        SetActiveLine(true);
    }

    private void OnMouseExit()
    {
        SetActiveLine(false);
    }
}