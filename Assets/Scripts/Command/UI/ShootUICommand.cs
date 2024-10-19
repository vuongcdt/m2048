using System.Collections.Generic;
using DG.Tweening;

public class ShootUICommand : CommandBase<bool>
{
    private List<Square> _squaresList = new();
    private Sequence _sequence;
    private StepAction _stepAction;
    private float _mergeDuration;
    private float _timeDelay;

    public ShootUICommand(List<Square> squaresList, Sequence sequence, StepAction stepAction, float mergeDuration, float timeDelay)
    {
        _squaresList = squaresList;
        _sequence = sequence;
        _stepAction = stepAction;
        _mergeDuration = mergeDuration;
        _timeDelay = timeDelay;
    }

    protected override void Init()
    {
    }

    public override bool Excute()
    {
        ShootUI();
        return true;
    }

    private void ShootUI()
    {
        _sequence.OnStart(() => Observer.Emit(Constants.EventKey.SOUND_SHOOT));
        var squarePool = new FindSquarePoolByIdCommand(_squaresList, _stepAction.singleSquareSources.id).Excute();

        squarePool.SetValue(_stepAction.newSquareValue);
        squarePool.transform.position = _stepAction.singleSquareSources.Position;

        _sequence.Append(squarePool.transform
            .DOMoveY(_stepAction.squareTarget.Position.y, _mergeDuration)
            .SetEase(Ease.Linear));

        _sequence.AppendInterval(_timeDelay);
    }
}
