using UnityEngine;

public class LineColumn : MonoBehaviour
{
    [SerializeField] private GameObject square;
    [SerializeField] private SpriteRenderer bg;
    
    private Square _squareScript;
    private BoardManager _boardManager;
    private PlayerManager _playerManager;

    private int _column;

    public int Column
    {
        get => _column;
        set => _column = value;
    }

    private void Awake()
    {
        _boardManager = BoardManager.Instance;
        _playerManager = PlayerManager.Instance;
    }

    private void OnMouseDown()
    {
        SetActiveLine(true);
        _boardManager.columnSelect = Column;
        _boardManager.isTouchLine = true;
    }

    private void OnMouseUp()
    {
        _playerManager.DeActiveLines();
        _boardManager.isTouchLine = false;
        _boardManager.ShootBlock();
    }

    public void SetActiveLine(bool value)
    {
        bg.color = value ? Color.red : new Color(0.27f, 0.27f, 0.27f, 0.35f);
    }

    private void OnMouseEnter()
    {
        if (!_boardManager.isTouchLine) return;
        _boardManager.columnSelect = Column;
        SetActiveLine(true);
    }

    private void OnMouseExit()
    {
        SetActiveLine(false);
    }
}