using System.Collections.Generic;
using UnityEngine;

public class InstanceNewSquareDataCommand : CommandBase<Square>
{
    private Transform _squareParentTransform;
    private List<Square> _squaresList = new();
    private Square _squareScript;
    private Vector3 _pos;

    public InstanceNewSquareDataCommand(Transform squareParentTransform, List<Square> squaresList, Square squareScript, Vector3 pos)
    {
        _squareParentTransform = squareParentTransform;
        _squareScript = squareScript;
        _squaresList = squaresList;
        _pos = pos;
    }
    protected override void Init()
    {
    }

    public override Square Excute()
    {
        return InstanceNewSquareData();
    }

    private Square InstanceNewSquareData()
    {
        var squarePool = FindSquarePoolDeActive();
        if (squarePool is null)
        {
            squarePool = Object.Instantiate(_squareScript, _pos, Quaternion.identity, _squareParentTransform);

            _squaresList.Add(squarePool);
        }
        else
        {
            squarePool.transform.position = _pos;
            squarePool.gameObject.SetActive(true);
        }

        return squarePool;
    }

    private Square FindSquarePoolDeActive()
    {
        for (var i = _squaresList.Count - 1; i >= 0; i--)
        {
            if (!_squaresList[i].gameObject.activeSelf)
            {
                return _squaresList[i];
            }
        }

        return null;
    }
}
