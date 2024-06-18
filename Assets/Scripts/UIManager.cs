﻿using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.Profiling;
using UnityEngine;
using uPools;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private GameObject squarePrefab;
    [SerializeField] private Transform squareParentTransform;

    private BoardManager _boardManager;
    private const float MERGE_DURATION = 0.1f;
    private const float TIME_DELAY = 0.1f;
    private List<Square> _squaresList = new();
    private List<BoardAction> _actionsWrapList = new();
    private Sequence _sequence;
    private int _idCount = 30;
    private int _comboCount;
    private int _score;
    private static readonly ProfilerMarker ProcessingTweenMaker = new("MyMaker.DOTweenSequence");

    public bool isPause;

    public void ToggleSequence()
    {
        _sequence.TogglePause();
    }

    public void UnsetPause()
    {
        isPause = false;
        _sequence.Play();
    }

    public void StartUI(List<SquareData> squaresData)
    {
        SharedGameObjectPool.Prewarm(squarePrefab.gameObject, 30);

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
        squaresData[4].value = 8;
        squaresData[5].value = 2;
        squaresData[11].value = 4;

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
        InitSquare();
        _comboCount = 0;
        _sequence = DOTween.Sequence();

        _actionsWrapList = actionsWrapList;
        
        foreach (var actionListWrap in _actionsWrapList)
        {
            Debug.Log("----actionListWrap: " + JsonUtility.ToJson(actionListWrap));
        }

        foreach (var actionListWrap in _actionsWrapList)
        {
            Debug.Log($"actionListWrap.actionType {actionListWrap.actionType}");
            switch (actionListWrap.actionType)
            {
                case ActionType.Shoot:
                    ShootUI(_sequence, actionListWrap.stepActionList.First());
                    break;
                case ActionType.MergeAllBlock:
                    MergeUI(_sequence, actionListWrap.stepActionList);
                    break;
                case ActionType.SortAllBlock:
                    SortUI(_sequence, actionListWrap.stepActionList);
                    break;
                case ActionType.ClearMinBlock:
                    ClearMinBlockUI(_sequence, actionListWrap.stepActionList);
                    break;
            }
        }

        _sequence.OnComplete(() =>
        {
            Debug.Log("OnComplete");
            _boardManager.isProcessing = false;
        });

        if (_comboCount > 2)
        {
            Debug.Log("////////// " + _comboCount);
        }
    }

    private void ClearMinBlockUI(Sequence sequence, List<StepAction> stepActionList)
    {
        Sequence clearSequence = DOTween.Sequence();
        foreach (var stepAction in stepActionList)
        {
            Debug.Log($"ClearMinBlockUI {JsonUtility.ToJson(stepAction)}");

        }

        // _squaresList.FindAll()
        
        // _squaresList.FindAll(squareGameObject =>
        //         stepActionList.First().squareSources
        //             .Any(sourceSquare => squareGameObject.squareData.id == sourceSquare.id))
        //     .ForEach(squareGameObject =>
        //     {
        //         clearSequence.Join(squareGameObject.transform.DOMove(Vector3.one, 0)
        //             .OnComplete(() =>
        //             {
        //                 squareGameObject.gameObject.SetActive(false);
        //                 squareGameObject.SetValue(0);
        //             }));
        //     });
        sequence.Append(clearSequence);
    }

    private void Start()
    {
        _boardManager = BoardManager.Instance;
    }

    private void SetPause()
    {
        if (isPause)
        {
            _sequence.Pause();
        }
    }

    private void ResetUI(List<SquareData> squaresData)
    {
        squaresData
            .FindAll(squareData => squareData.value > 0)
            .ForEach(squareData =>
            {
                var newSquareData = SharedGameObjectPool.Rent(squarePrefab, squareData.Position, Quaternion.identity,
                    squareParentTransform);
                var newSquareComp = newSquareData.GetComponent<Square>();
                newSquareComp.SetValue(squareData.value);
                newSquareComp.SetId(squareData.cell.Column + 1 + squareData.cell.Row * 6); // todo
                newSquareComp.instantPool = newSquareData;
                _squaresList.Add(newSquareComp);
            });
    }

    private void ShootUI(Sequence sequence, StepAction stepAction)
    {
        // Debug.Log($"ShootUI {JsonUtility.ToJson(stepAction)}");

        var squarePool = _squaresList.Find(squareGameObject =>
            squareGameObject.squareData.id == stepAction.singleSquareSources.id);
        
        squarePool.SetValue(stepAction.newSquareValue);
        squarePool.transform.position = stepAction.singleSquareSources.Position;

        sequence.Append(squarePool.transform
                .DOMoveY(stepAction.squareTarget.Position.y, MERGE_DURATION)
                .SetEase(Ease.Linear))
            .OnComplete(() =>
            {
                // Debug.Log("SHOOT");
                SetPause();
            });
        sequence.AppendInterval(TIME_DELAY);
    }

    private void InitSquare()
    {
        // var squarePool = _squaresList.Find(squareGameObject => !squareGameObject.gameObject.activeSelf);
        _idCount++;
        var newSquarePos = new Vector3(0, 6, 0);

        var squarePool =
            SharedGameObjectPool.Rent(squarePrefab, newSquarePos, Quaternion.identity, squareParentTransform);
        var newSquareComp = squarePool.GetComponent<Square>();
        newSquareComp.instantPool = squarePool;

        newSquareComp.SetId(_idCount);
        _squaresList.Add(newSquareComp);
    }

    private void MergeUI(Sequence sequence, List<StepAction> mergerActionList)
    {
        Sequence mergerSequence = DOTween.Sequence();
        _comboCount++;

        foreach (var mergerAction in mergerActionList)
        {
            _boardManager.score += mergerAction.newSquareValue;

            // Debug.Log($"MergeUI  {JsonUtility.ToJson(mergerAction)}");
            var squareSourceGameObjectsList = FindAllSquareGameObjectsActiveSameValue(mergerAction);
            var squareTargetGameObject = FindSquareGameObjectActiveById(mergerAction.squareTarget.id);

            foreach (var squareSourceGameObject in squareSourceGameObjectsList)
            {
                mergerSequence.Join(squareSourceGameObject.transform
                    .DOMove(mergerAction.squareTarget.Position, MERGE_DURATION)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        // Debug.Log("MERGE");
                        squareTargetGameObject.SetValue(mergerAction.newSquareValue);
                        squareSourceGameObject.SetValue(0);
                        squareSourceGameObject.ReturnPool();
                        SetPause();
                    })
                );
            }
        }

        sequence.Append(mergerSequence);
        sequence.AppendInterval(TIME_DELAY);
    }

    private void SortUI(Sequence sequence, List<StepAction> mergerActionList)
    {
        Sequence sortSequence = DOTween.Sequence();

        foreach (var mergerAction in mergerActionList)
        {
            // Debug.Log($"SortUI_  {JsonUtility.ToJson(mergerAction)}");

            var squareSourceGameObject = FindSquareGameObjectActiveById(mergerAction.singleSquareSources.id);

            sortSequence.Join(squareSourceGameObject.transform
                .DOMove(mergerAction.squareTarget.Position, MERGE_DURATION)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    // Debug.Log("SORT");
                    squareSourceGameObject.SetValue(mergerAction.newSquareValue);
                    SetPause();
                })
            );
        }

        sequence.Append(sortSequence);
        sequence.AppendInterval(TIME_DELAY);
    }

    private List<Square> FindAllSquareGameObjectsActiveSameValue(StepAction stepAction)
    {
        return _squaresList.FindAll(squareGameObj =>
            squareGameObj.gameObject.activeSelf &&
            IsCompareSquareActiveSameId(stepAction.multiSquareSources, squareGameObj));
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