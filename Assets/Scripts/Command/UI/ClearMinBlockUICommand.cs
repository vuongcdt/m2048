
using System.Collections.Generic;
using DG.Tweening;

public class ClearMinBlockUICommand : CommandBase<bool>
{
    private List<Square> _squaresList = new();
    private List<StepAction> _stepActionList = new();
    private Sequence _sequence;

    public ClearMinBlockUICommand(List<Square> squaresList, List<StepAction> stepActionList, Sequence sequence)
    {
        _squaresList = squaresList;
        _stepActionList = stepActionList;
        _sequence = sequence;
    }

    protected override void Init()
    {
    }

    public override bool Excute()
    {
        ClearMinBlockUI();
        return true;
    }

    private void ClearMinBlockUI()
    {
        Sequence clearMinValueSequence = DOTween.Sequence();

        foreach (var squareGameObject in _squaresList)
        {
            foreach (var stepAction in _stepActionList)
            {
                if (stepAction.squareTarget.id != squareGameObject.squareData.id)
                {
                    continue;
                }

                clearMinValueSequence.Join(squareGameObject.transform
                    .DOMove(stepAction.squareTarget.Position, 0)
                    .OnComplete(() =>
                    {
                        squareGameObject.SetValue(0);
                        squareGameObject.ReturnPool();
                    }));
            }
        }

        _sequence.Append(clearMinValueSequence);
    }


}
