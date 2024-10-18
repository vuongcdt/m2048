using System.Collections.Generic;
using UnityEngine;

public class RenderUICommand : CommandBase<bool>
{
    private Transform _squareParentTransform;
    private List<Square> _squaresList = new();
    private Square _squareScript;
    private List<SquareData> _squaresData;

    public RenderUICommand(Transform squareParentTransform, List<Square> squaresList, Square squareScript, List<SquareData> squaresData)
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
        // RenderUI();
        return true;
    }

    // public void RenderUI(List<BoardAction> actionsWrapList)
    // {
    //     _actionsWrapList = actionsWrapList;

    //     if (_actionsWrapList.Count <= 0)
    //     {
    //         _boardManager.isProcessing = false;
    //         return;
    //     }

    //     InitNewSquareForShoot();
    //     comboCount = 0;
    //     _sequence = DOTween.Sequence();

    //     foreach (var actionListWrap in _actionsWrapList)
    //     {
    //         switch (actionListWrap.actionType)
    //         {
    //             case ActionType.Shoot:
    //                 ShootUI(_sequence, actionListWrap.stepActionList.First());
    //                 break;
    //             case ActionType.MergeAllBlock:
    //                 MergeUI(_sequence, actionListWrap.stepActionList);
    //                 break;
    //             case ActionType.SortAllBlock:
    //                 SortUI(_sequence, actionListWrap.stepActionList);
    //                 break;
    //             case ActionType.ClearMinBlock:
    //                 ClearMinBlockUI(_sequence, actionListWrap.stepActionList);
    //                 break;
    //         }
    //     }

    //     _sequence.OnComplete(() =>
    //     {
    //         _boardManager.isProcessing = false;
    //         SetComboUI();

    //         if (_boardManager.isGameOver)
    //         {
    //             SetGameOverUI();
    //         }
    //     });
    // }


    // private void SetComboUI()
    // {
    //     if (comboCount > 2)
    //     {
    //         _soundManager.PlaySoundComboSfx();

    //         Observer.Emit(Constants.EventKey.COMBO, new ComboEvent(comboCount, comboPos));

    //         var endNewValueMerge = _actionsWrapList
    //             .Where(boardAction => boardAction.actionType == ActionType.MergeAllBlock)
    //             .Select(boardAction => boardAction.stepActionList[^1].newSquareValue)
    //             .Last();

    //         _boardManager.score += comboCount * endNewValueMerge;
    //         SetScoreUI();
    //     }
    // }

    // private void SetGameOverUI()
    // {
    //     idCount = 30;
    //     _soundManager.PlaySoundGameOverSfx();
    //     _boardManager.isProcessing = true;
    //     Observer.Emit(Constants.EventKey.GAME_OVER_POPUP);
    // }

    // private void ShootUI(Sequence sequence, StepAction stepAction)
    // {
    //     sequence.OnStart(_soundManager.PlaySoundShootSfx);
    //     var squarePool = FindSquarePoolById(stepAction.singleSquareSources.id);

    //     squarePool.SetValue(stepAction.newSquareValue);
    //     squarePool.transform.position = stepAction.singleSquareSources.Position;

    //     sequence.Append(squarePool.transform
    //         .DOMoveY(stepAction.squareTarget.Position.y, MERGE_DURATION)
    //         .SetEase(Ease.Linear));

    //     sequence.AppendInterval(TIME_DELAY);
    // }

    // private Square FindSquarePoolById(int id)
    // {
    //     for (var i = _squaresList.Count - 1; i >= 0; i--)
    //     {
    //         if (_squaresList[i].squareData.id == id)
    //         {
    //             return _squaresList[i];
    //         }
    //     }

    //     return null;
    // }

    // private void InitNewSquareForShoot()
    // {
    //     idCount++;
    //     var newSquarePos = new Vector3(0, -10, 0);

    //     var instanceNewSquareDataCommand = new InstanceNewSquareDataCommand(squareParentTransform, _squaresList, _squareScript, newSquarePos);
    //     var squarePool = instanceNewSquareDataCommand.Excute();

    //     squarePool.SetId(idCount);
    // }

    // private void MergeUI(Sequence sequence, List<StepAction> mergerActionList)
    // {
    //     Sequence mergerSequence = DOTween.Sequence();
    //     comboCount++;

    //     mergerSequence.OnStart(_soundManager.PlaySoundMergeSfx);
    //     foreach (var mergerAction in mergerActionList)
    //     {
    //         var squareSourceGameObjectsList = FindAllSquareGameObjectsActiveSameValue(mergerAction);
    //         var squareTargetGameObject = FindSquarePoolById(mergerAction.squareTarget.id);

    //         foreach (var squareSourceGameObject in squareSourceGameObjectsList)
    //         {
    //             mergerSequence.Join(squareSourceGameObject.transform
    //                 .DOMove(mergerAction.squareTarget.Position, MERGE_DURATION)
    //                 .SetEase(Ease.Linear)
    //                 .OnComplete(() =>
    //                 {
    //                     squareTargetGameObject.SetValue(mergerAction.newSquareValue);
    //                     squareSourceGameObject.SetValue(0);
    //                     squareSourceGameObject.ReturnPool();
    //                 })
    //             );
    //         }

    //         mergerSequence.OnComplete(() =>
    //         {
    //             comboPos = mergerAction.squareTarget.Position;
    //             _boardManager.score += mergerAction.newSquareValue;
    //             SetScoreUI();
    //         });
    //     }

    //     sequence.Append(mergerSequence);
    //     sequence.AppendInterval(TIME_DELAY);
    // }

    // private List<Square> FindAllSquareGameObjectsActiveSameValue(StepAction stepAction)
    // {
    //     List<Square> list = new List<Square>();
    //     foreach (var squareGameObj in _squaresList)
    //     {
    //         if (IsCompareSquareActiveSameId(stepAction.multiSquareSources, squareGameObj))
    //         {
    //             list.Add(squareGameObj);
    //         }
    //     }

    //     return list;
    // }

    // private bool IsCompareSquareActiveSameId(List<SquareData> squareSources, Square squareGameObj)
    // {
    //     var isSquareActiveSameIndex = false;
    //     foreach (var squareData in squareSources)
    //     {
    //         if (squareData.id == squareGameObj.squareData.id && squareGameObj.gameObject.activeSelf)
    //         {
    //             isSquareActiveSameIndex = true;
    //             break;
    //         }
    //     }

    //     return isSquareActiveSameIndex;
    // }

    // public void SetScoreUI(GamePlayScreen gamePlayScreen = null)
    // {
    //     _boardManager = BoardManager.Instance;
    //     if (_boardManager.score > _boardManager.highScore)
    //     {
    //         _boardManager.highScore = _boardManager.score;
    //     }

    //     var data = new ScoreDataEvent()
    //     {
    //         highScore = _boardManager.highScore,
    //         score = _boardManager.score,
    //     };
    //     Observer.Emit(Constants.EventKey.SCORE, data);
    // }

    // private void SortUI(Sequence sequence, List<StepAction> mergerActionList)
    // {
    //     sequence.OnStart(_soundManager.PlaySoundSortSfx);
    //     Sequence sortSequence = DOTween.Sequence();

    //     foreach (var mergerAction in mergerActionList)
    //     {
    //         var squareSourceGameObject = FindSquarePoolById(mergerAction.singleSquareSources.id);

    //         sortSequence.Join(squareSourceGameObject.transform
    //             .DOMove(mergerAction.squareTarget.Position, MERGE_DURATION)
    //             .SetEase(Ease.Linear)
    //             .OnComplete(() =>
    //             {
    //                 var mergeEndPos = mergerAction.singleSquareSources.Position;
    //                 squareSourceGameObject.SetValue(mergerAction.newSquareValue);
    //                 if ((mergeEndPos - comboPos).sqrMagnitude == 0)
    //                 {
    //                     comboPos = mergerAction.squareTarget.Position;
    //                 }
    //             })
    //         );
    //     }

    //     sequence.Append(sortSequence);
    //     sequence.AppendInterval(TIME_DELAY);
    // }

    // private void ClearMinBlockUI(Sequence sequence, List<StepAction> stepActionList)
    // {
    //     Sequence clearMinValueSequence = DOTween.Sequence();

    //     foreach (var squareGameObject in _squaresList)
    //     {
    //         foreach (var stepAction in stepActionList)
    //         {
    //             if (stepAction.squareTarget.id != squareGameObject.squareData.id)
    //             {
    //                 continue;
    //             }

    //             clearMinValueSequence.Join(squareGameObject.transform
    //                 .DOMove(stepAction.squareTarget.Position, 0)
    //                 .OnComplete(() =>
    //                 {
    //                     squareGameObject.SetValue(0);
    //                     squareGameObject.ReturnPool();
    //                 }));
    //         }
    //     }

    //     sequence.Append(clearMinValueSequence);
    // }

}
