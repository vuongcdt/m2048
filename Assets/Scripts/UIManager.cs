using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class UIManager:Singleton<UIManager>
{
    [SerializeField] private Square square;
    [SerializeField] private Transform squareParentTransform;
    
    private const float MERGE_DURATION = 0.5f;
    private List<Square> _squaresList = new();
    private List<BoardAction> _actionsWrapList = new();

    public void RenderStartUI(List<SquareData> squaresData)
    {
        // squaresData[0].value = 2;
        // squaresData[6].value = 4;

        squaresData[0].value = 16;
        squaresData[1].value = 2;
        squaresData[2].value = 16;

        squaresData[6].value = 2;
        // squaresData[7].value = 2;
        squaresData[8].value = 2;

        squaresData[12].value = 64;
        // var cellCheck = GetSquareDataByCell(new Utils.Cell(1, 1));


        // Shoot(1);

        ////

        // squaresData[2].value = 2;
        // // squaresData[8].value = 2;
        // Shoot(3);

        // var cellCheck = GetSquareDataByCell(new Utils.Cell(2, 1));
        // _processingSquare = cellCheck;

        // squaresData[0].value = 2;
        // squaresData[1].value = 8;


        // squaresData[2].value = 2;
        //
        // squaresData[6].value = 8;
        // squaresData[7].value = 2;
        // // squaresData[8].value = 2;
        //
        // squaresData[12].value = 4;
        // squaresData[13].value = 8;
        //
        // squaresData[19].value = 4;
        // // var cellCheck = GetSquareDataByCell(new Utils.Cell(2, 1));
        // ////
        // Shoot(2);
        
        squaresData.FindAll(squareData => squareData.value > 0)
            .ForEach(squareData =>
            {
                var newSquareData = Instantiate(square, squareData.Position, Quaternion.identity);
                newSquareData.SetValue(squareData.value);
                newSquareData.SetIndex(squareData.index);
                
                _squaresList.Add(newSquareData);
            });
    }
    
    public void RenderUI(List<BoardAction> actionsWrapList)
    {
        _actionsWrapList = actionsWrapList;
        
        Sequence sequence = DOTween.Sequence().Pause();

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


        sequence.Play();
    }

    private void ShootUI(Sequence sequence, StepAction stepAction)
    {
        Debug.Log($"ShootUI {JsonUtility.ToJson(stepAction)}");

        var duration = GetDurationMove(stepAction.squareSources[0].Position, stepAction.squareTarget.Position);

        var newSquare = Instantiate(square,
            stepAction.squareSources[0].Position,
            Quaternion.identity,
            squareParentTransform);
        newSquare.SetIndex(stepAction.squareTarget.index);
        newSquare.SetValue(stepAction.newSquareValue);

        _squaresList.Add(newSquare);
        sequence.Append(newSquare.transform
            .DOMoveY(stepAction.squareTarget.Position.y, duration)
            .SetEase(Ease.Linear));
    }

    private void MergeUI(Sequence sequence, List<StepAction> mergerActionList)
    {
        Sequence mergerSequence = DOTween.Sequence();

        foreach (var mergerAction in mergerActionList)
        {
            Debug.Log($"MergeUI  {JsonUtility.ToJson(mergerAction)}");
            var squareSourceGameObjectsList = FindAllSquareGameObjectsSameValueActive(mergerAction);
            var squareTargetGameObject = FindSquareGameObjectActiveByIndex(mergerAction.squareTarget.index);

            foreach (var squareSourceGameObject in squareSourceGameObjectsList)
            {
                mergerSequence.Join(squareSourceGameObject.transform
                    .DOMove(mergerAction.squareTarget.Position, MERGE_DURATION)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        squareTargetGameObject.SetValue(mergerAction.newSquareValue);
                        squareSourceGameObject.squareData.value = 0;
                        squareSourceGameObject.squareData.index = -1;
                        squareSourceGameObject.DeActive();
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

            var squareSourceGameObject = FindSquareGameObjectActiveByIndex(mergerAction.squareSources[0].index);

            sortSequence.Join(squareSourceGameObject.transform
                .DOMoveY(mergerAction.squareTarget.Position.y, MERGE_DURATION)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    squareSourceGameObject.SetValue(mergerAction.newSquareValue);
                    squareSourceGameObject.SetIndex(mergerAction.squareTarget.index);
                })
            );
        }

        sequence.Append(sortSequence);
    }

    private List<Square> FindAllSquareGameObjectsSameValueActive(StepAction stepAction)
    {
        return _squaresList.FindAll(squareGameObj =>
            squareGameObj.gameObject.activeSelf &&
            IsCompareSquareSameIndex(stepAction.squareSources, squareGameObj));
    }

    private Square FindSquareGameObjectActiveByIndex(int index)
    {
        return _squaresList.Find(squareGameObj =>
            squareGameObj.squareData.index == index && squareGameObj.gameObject.activeSelf);
    }

    private bool IsCompareSquareSameIndex(List<SquareData> squareSources, Square squareGameObj)
    {
        var isSquareSamePosition = squareSources.Any(squareData => squareData.index == squareGameObj.squareData.index);
        return isSquareSamePosition;
    }


    private float GetDurationMove(Vector2 posSource, Vector2 posTarget)
    {
        var distance = Vector2.Distance(posSource, posTarget);
        return (distance / 4 + 3) / 10;
    }

}