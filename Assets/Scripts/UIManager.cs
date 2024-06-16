using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private Square square;
    [SerializeField] private Transform squareParentTransform;

    private const float MERGE_DURATION = 0.2f;
    private List<Square> _squaresList = new();
    private List<BoardAction> _actionsWrapList = new();
    private Tween _sequence;

    public void ToggleSequence()
    {
        _sequence.TogglePause();
    }

    public void RewindSequence()
    {
        _sequence.Rewind();
    }

    public void StartUI(List<SquareData> squaresData)
    {
        // squaresData[0].value = 2;
        // squaresData[6].value = 4;

        ////
        squaresData[0].value = 16;
        squaresData[1].value = 2;
        squaresData[2].value = 16;

        squaresData[6].value = 2;
        squaresData[8].value = 2;

        squaresData[12].value = 64;

        ////
        // squaresData[2].value = 2;

        ////
        // squaresData[0].value = 2;
        // squaresData[1].value = 8;
        // squaresData[2].value = 2;
        //
        // squaresData[6].value = 8;
        // squaresData[7].value = 2;
        //
        // squaresData[12].value = 4;
        // squaresData[13].value = 8;
        //
        // squaresData[19].value = 4;

        ////
        // squaresData[0].value = 2;
        // squaresData[6].value = 4;
        // squaresData[12].value = 2;

        ////
        // squaresData[6].value = 8;
        // squaresData[7].value = 2;

        ResetUI(squaresData);
    }

    public void RenderUI(List<BoardAction> actionsWrapList)
    {
        _actionsWrapList = actionsWrapList;

        Sequence sequence = DOTween.Sequence();
        _sequence = sequence;

        foreach (var actionListWrap in _actionsWrapList)
        {
            switch (actionListWrap.actionType)
            {
                case ActionType.Shoot:
                    ShootUI(sequence, actionListWrap.stepActionList.First());
                    break;
                case ActionType.MergeAllBlock:
                    MergeUI(sequence, actionListWrap.stepActionList);
                    break;
                case ActionType.SortAllBlock:
                    SortUI(sequence, actionListWrap.stepActionList);
                    break;
            }
        }
    }

    private void ResetUI(List<SquareData> squaresData)
    {
        squaresData
            .FindAll(squareData => squareData.value > 0)
            .ForEach(squareData =>
            {
                var newSquareData =
                    Instantiate(square, squareData.Position, Quaternion.identity, squareParentTransform);
                newSquareData.SetValue(squareData.value);
                newSquareData.SetId(squareData.cell.Column + 1 + squareData.cell.Row * 6); // todo

                _squaresList.Add(newSquareData);
            });
    }

    private void ShootUI(Sequence sequence, StepAction stepAction)
    {
        Debug.Log($"ShootUI {JsonUtility.ToJson(stepAction)}");

        var duration = GetDurationMove(stepAction.squareSources[0].Position, stepAction.squareTarget.Position);

        var squarePool = _squaresList.Find(square => !square.gameObject.activeSelf);
        if (squarePool != null)
        {
            squarePool.SetId(stepAction.squareTarget.id);

            squarePool.SetValue(stepAction.newSquareValue);
            squarePool.transform.position = stepAction.squareSources[0].Position;
            squarePool.SetActive(true);
        }
        else
        {
            squarePool = Instantiate(square,
                stepAction.squareSources[0].Position,
                Quaternion.identity,
                squareParentTransform);

            squarePool.SetId(stepAction.squareTarget.id);
            squarePool.SetValue(stepAction.newSquareValue);

            _squaresList.Add(squarePool);
        }


        sequence.Append(squarePool.transform
            .DOMoveY(stepAction.squareTarget.Position.y, duration)
            .SetEase(Ease.Linear));
    }

    private void MergeUI(Sequence sequence, List<StepAction> mergerActionList)
    {
        Sequence mergerSequence = DOTween.Sequence();

        foreach (var mergerAction in mergerActionList)
        {
            Debug.Log($"MergeUI  {JsonUtility.ToJson(mergerAction)}");
            var squareSourceGameObjectsList = FindAllSquareGameObjectsActiveSameValue(mergerAction);
            var squareTargetGameObject = FindSquareGameObjectActiveById(mergerAction.squareTarget.id);

            foreach (var squareSourceGameObject in squareSourceGameObjectsList)
            {
                mergerSequence.Join(squareSourceGameObject.transform
                    .DOMove(mergerAction.squareTarget.Position, MERGE_DURATION)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        squareTargetGameObject.SetValue(mergerAction.newSquareValue);
                        squareSourceGameObject.SetValue(0);
                        squareSourceGameObject.SetActive(false);

                        sequence.Pause();
                    })
                );
            }
        }

        sequence.Append(mergerSequence);
    }

    private void SortUI(Sequence sequence, List<StepAction> mergerActionList)
    {
        Sequence sortSequence = DOTween.Sequence();

        foreach (var mergerAction in mergerActionList)
        {
            Debug.Log($"SortUI_  {JsonUtility.ToJson(mergerAction)}");

            var squareSourceGameObject = FindSquareGameObjectActiveById(mergerAction.squareSources[0].id);

            sortSequence.Join(squareSourceGameObject.transform
                .DOMove(mergerAction.squareTarget.Position, MERGE_DURATION)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    squareSourceGameObject.SetValue(mergerAction.newSquareValue);
                    sequence.Pause();
                })
            );
        }

        sequence.Append(sortSequence);
    }

    private List<Square> FindAllSquareGameObjectsActiveSameValue(StepAction stepAction) // check lai
    {
        return _squaresList.FindAll(squareGameObj =>
            squareGameObj.gameObject.activeSelf &&
            IsCompareSquareActiveSameId(stepAction.squareSources, squareGameObj));
    }

    private Square FindSquareGameObjectActiveById(int id)
    {
        return _squaresList.Find(squareGameObj =>
            squareGameObj.squareData.id == id && squareGameObj.gameObject.activeSelf);
    }

    private bool IsCompareSquareActiveSameId(List<SquareData> squareSources, Square squareGameObj)
    {
        var isSquareActiveSameIndex = squareSources.Any(squareData =>
            squareData.id == squareGameObj.squareData.id && squareGameObj.gameObject.activeSelf);
        return isSquareActiveSameIndex;
    }


    private float GetDurationMove(Vector2 posSource, Vector2 posTarget)
    {
        var distance = Vector2.Distance(posSource, posTarget);
        return (distance / 4 + 3) / 10;
    }
}