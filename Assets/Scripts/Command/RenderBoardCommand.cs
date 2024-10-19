
public class RenderBoardCommand : CommandBase<bool>
{
    private BoardManager _boardManager;

    public RenderBoardCommand()
    {
        _boardManager = BoardManager.Instance;
    }

    protected override void Init()
    {
    }

    public override bool Excute()
    {
        RenderBoard();
        return true;
    }

    private void RenderBoard()
    {
        _boardManager.squaresData = new();
        for (var y = _boardManager.boardRow; y > 0; y--)
        {
            for (var x = 0; x < _boardManager.boardCol; x++)
            {
                _boardManager.idCount++;
                _boardManager.squaresData.Add(new SquareData(
                    new Utils.Cell(x, _boardManager.boardRow - y),
                    _boardManager.idCount,
                    0));
            }
        }
    }
}
