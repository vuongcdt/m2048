using System.Collections.Generic;
using DG.Tweening;

public class MergeUICommand : CommandBase<bool>
{
    private List<Square> _squaresList = new();
    private List<StepAction> _mergerActionList = new();
    private Sequence _sequence;
    private float _mergeDuration;
    private float _timeDelay;
    private UIManager _uiManager;
    private BoardManager _boardManager;

    public MergeUICommand(List<Square> squaresList, Sequence sequence, List<StepAction> mergerActionList, float mergeDuration, float timeDelay)
    {
        _squaresList = squaresList;
        _mergerActionList = mergerActionList;
        _sequence = sequence;
        _mergeDuration = mergeDuration;
        _timeDelay = timeDelay;
        _uiManager = UIManager.Instance;
        _boardManager = BoardManager.Instance;
    }

    protected override void Init()
    {
    }

    public override bool Excute()
    {
        MergeUI();
        return true;
    }

    private void MergeUI()
    {
        Sequence mergerSequence = DOTween.Sequence();
        _uiManager.comboCount++;

        mergerSequence.OnStart(() => Observer.Emit(Constants.EventKey.SOUND_MERGE));
        foreach (var mergerAction in _mergerActionList)
        {
            var squareSourceGameObjectsList = new FindAllSquareGameObjectsActiveSameValueCommand(_squaresList, mergerAction).Excute();
            var squareTargetGameObject = new FindSquarePoolByIdCommand(_squaresList, mergerAction.squareTarget.id).Excute();

            foreach (var squareSourceGameObject in squareSourceGameObjectsList)
            {
                mergerSequence.Join(squareSourceGameObject.transform
                    .DOMove(mergerAction.squareTarget.Position, _mergeDuration)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        squareTargetGameObject.SetValue(mergerAction.newSquareValue);
                        squareSourceGameObject.SetValue(0);
                        squareSourceGameObject.ReturnPool();
                    })
                );
            }

            mergerSequence.OnComplete(() =>
            {
                _uiManager.comboPos = mergerAction.squareTarget.Position;
                _boardManager.score += mergerAction.newSquareValue;
                Observer.Emit(Constants.EventKey.SET_SCORE_UI);
            });
        }

        _sequence.Append(mergerSequence);
        _sequence.AppendInterval(_timeDelay);
    }
}
