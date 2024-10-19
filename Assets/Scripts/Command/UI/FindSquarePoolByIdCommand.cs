using System.Collections.Generic;

public class FindSquarePoolByIdCommand : CommandBase<Square>
{
    private List<Square> _squaresList = new();
    private int _id;

    public FindSquarePoolByIdCommand(List<Square> squaresList, int id)
    {
        _squaresList = squaresList;
        _id = id;
    }
    protected override void Init()
    {
    }

    public override Square Excute()
    {
        return FindSquarePoolById();
    }

    private Square FindSquarePoolById()
    {
        for (var i = _squaresList.Count - 1; i >= 0; i--)
        {
            if (_squaresList[i].squareData.id == _id)
            {
                return _squaresList[i];
            }
        }

        return null;
    }
}
