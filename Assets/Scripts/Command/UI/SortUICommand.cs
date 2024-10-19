using System.Collections.Generic;
using DG.Tweening;

public class SortUICommand : CommandBase<bool>
{
    private List<Square> _squaresList = new();
    private List<StepAction> _mergerActionList = new();
    private Sequence _sequence;
    private float _mergeDuration;
    private float _timeDelay;
    private UIManager _uiManager;

    public SortUICommand(List<Square> squaresList, Sequence sequence, List<StepAction> mergerActionList, float mergeDuration, float timeDelay)
    {
        _squaresList = squaresList;
        _mergerActionList = mergerActionList;
        _sequence = sequence;
        _mergeDuration = mergeDuration;
        _timeDelay = timeDelay;
        _uiManager = UIManager.Instance;
    }

    protected override void Init()
    {
    }

    public override bool Excute()
    {
        SortUI();
        return true;
    }

    private void SortUI()
    {
        Sequence sortSequence = DOTween.Sequence();

        foreach (var mergerAction in _mergerActionList)
        {
            var squareSourceGameObject = new FindSquarePoolByIdCommand(_squaresList, mergerAction.singleSquareSources.id).Excute();

            sortSequence.Join(squareSourceGameObject.transform
                .DOMove(mergerAction.squareTarget.Position, _mergeDuration)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    var mergeEndPos = mergerAction.singleSquareSources.Position;
                    squareSourceGameObject.SetValue(mergerAction.newSquareValue);

                    if ((mergeEndPos - _uiManager.comboPos).sqrMagnitude == 0)
                    {
                        _uiManager.comboPos = mergerAction.squareTarget.Position;
                    }
                })
            );
        }

        _sequence.Append(sortSequence);
        _sequence.AppendInterval(_timeDelay);
    }
}
