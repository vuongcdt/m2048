using System.Collections.Generic;

public class FindAllSquareGameObjectsActiveSameValueCommand : CommandBase<List<Square>>
{
    private List<Square> _squaresList = new();
    private StepAction _stepAction;

    public FindAllSquareGameObjectsActiveSameValueCommand(List<Square> squaresList, StepAction stepAction)
    {
        _squaresList = squaresList;
        _stepAction = stepAction;
    }
    protected override void Init()
    {
    }

    public override List<Square> Excute()
    {
        return FindAllSquareGameObjectsActiveSameValue();
    }

    private List<Square> FindAllSquareGameObjectsActiveSameValue()
    {
        List<Square> list = new List<Square>();
        foreach (var squareGameObj in _squaresList)
        {
            if (IsCompareSquareActiveSameId(_stepAction.multiSquareSources, squareGameObj))
            {
                list.Add(squareGameObj);
            }
        }

        return list;
    }

    private bool IsCompareSquareActiveSameId(List<SquareData> squareSources, Square squareGameObj)
    {
        var isSquareActiveSameIndex = false;
        foreach (var squareData in squareSources)
        {
            if (squareData.id == squareGameObj.squareData.id && squareGameObj.gameObject.activeSelf)
            {
                isSquareActiveSameIndex = true;
                break;
            }
        }

        return isSquareActiveSameIndex;
    }
}
