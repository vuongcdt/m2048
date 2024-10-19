using System.Collections.Generic;
using UnityEngine;

public class ResetUICommnad : CommandBase<bool>
{
    private Transform _squareParentTransform;
    private List<Square> _squaresList = new();
    private Square _squareScript;
    private List<SquareData> _squaresData;

    public ResetUICommnad(Transform squareParentTransform, List<Square> squaresList, Square squareScript, List<SquareData> squaresData)
    {
        _squareParentTransform = squareParentTransform;
        _squareScript = squareScript;
        _squaresList = squaresList;
        _squaresData = squaresData;
    }

    protected override void Init()
    {
    }

    public override bool Excute()
    {
        ResetUI();
        return true;
    }

    private void ResetUI()
    {
        foreach (var squareData in _squaresData)
        {
            if (squareData.value <= 0)
            {
                continue;
            }

            var instanceNewSquareDataCommand = new InstanceNewSquareDataCommand(_squareParentTransform, _squaresList, _squareScript, squareData.Position);
            var newSquareData = instanceNewSquareDataCommand.Excute();

            newSquareData.SetValue(squareData.value);
            newSquareData.SetId(squareData.id);
        }
    }
}
