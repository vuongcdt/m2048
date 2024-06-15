using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class BoardManager : Singleton<BoardManager>
{
    #region NewCode

    [SerializeField] private List<SquareData> squareDatas = new();
    [SerializeField] private Square square;
    [SerializeField] private GameObject lineColumn;
    [SerializeField] private Transform lineParentTransform;
    [SerializeField] private Transform squareParentTransform;
    [SerializeField] private Square nextSquare;

    public int boardRow = 5;
    public int boardCol = 6;
    public bool isTouchLine;
    public int columnSelect;

    private const float MERGE_DURATION = 0.5f;
    private int _randomNum;
    private int _newSquareValue;
    private SquareData _processingSquare;
    private List<Square> _squaresList = new();
    private List<GameObject> _lineColumnList = new();
    private List<MergerAction> _actionsList = new();
    private List<MergerActionWrap> _actionsWrapList = new();
    private List<int> _squareValueList = new() { 2, 4 };
    // private readonly List<int> _listSquareValue = new() { 2, 4, 8, 16, 32, 64, 128 };

    private void Start()
    {
        ResetBoard();
        RenderLineColumn();
        SetRandomSquareValue();

        // ShootBlock(1);
        // TestData();
    }

    private void TestData()
    {
        // squareDatas[0].value = 2;
        // squareDatas[6].value = 4;

        squareDatas[0].value = 16;
        squareDatas[1].value = 2;
        squareDatas[2].value = 16;

        squareDatas[6].value = 2;
        // squareDatas[7].value = 2;
        squareDatas[8].value = 2;

        squareDatas[12].value = 64;
        // var cellCheck = GetSquareDataByCell(new Utils.Cell(1, 1));


        // Shoot(1);

        ////

        // squareDatas[2].value = 2;
        // // squareDatas[8].value = 2;
        // Shoot(3);

        // var cellCheck = GetSquareDataByCell(new Utils.Cell(2, 1));
        // _processingSquare = cellCheck;

        // squareDatas[0].value = 2;
        // squareDatas[1].value = 8;


        // squareDatas[2].value = 2;
        //
        // squareDatas[6].value = 8;
        // squareDatas[7].value = 2;
        // // squareDatas[8].value = 2;
        //
        // squareDatas[12].value = 4;
        // squareDatas[13].value = 8;
        //
        // squareDatas[19].value = 4;
        // // var cellCheck = GetSquareDataByCell(new Utils.Cell(2, 1));
        // ////
        // Shoot(2);
    }

    public void ShootBlock(int column)
    {
        _actionsWrapList = new();
        ProcessingData(column);
        RenderUI();

        // Debug.Log("------");
        // foreach (var actionListWrap in _actionsWrapList)
        // {
        //     Debug.Log(JsonUtility.ToJson(actionListWrap));
        // }
        // Debug.Log("......");

        SetRandomSquareValue();
    }

    private void RenderUI()
    {
        Sequence sequence = DOTween.Sequence();
        // var combineMergeActionList = CombineMergeActionList();

        foreach (var actionListWrap in _actionsWrapList)
        {
            switch (actionListWrap.actionType)
            {
                case ActionType.Shoot:
                    ShotUI(sequence, actionListWrap.mergerActionList.First());
                    break;
                case ActionType.MergeAllBlock:
                    MergeUI(sequence, actionListWrap.mergerActionList);
                    break;
                case ActionType.SortAllBlock:
                    SortUI(sequence, actionListWrap.mergerActionList);
                    break;
            }
        }
    }

    private void ShotUI(Sequence sequence, MergerAction mergerAction)
    {
        var duration = GetDurationMove(mergerAction.squareSources[0].Position, mergerAction.squareTarget.Position);

        var newSquare = Instantiate(square,
            mergerAction.squareSources[0].Position,
            Quaternion.identity,
            squareParentTransform);
        newSquare.SetIndex(mergerAction.squareTarget.index);
        newSquare.SetValue(mergerAction.newSquareValue);

        _squaresList.Add(newSquare);
        sequence.Append(newSquare.transform
            .DOMoveY(mergerAction.squareTarget.Position.y, duration)
            .SetEase(Ease.Linear));
    }

    private void MergeUI(Sequence sequence, List<MergerAction> mergerActionList)
    {
        Debug.Log("Merger BLOCK");
        ChangeAction(sequence);

        foreach (var mergerAction in mergerActionList)
        {
            Debug.Log($"Merger {JsonUtility.ToJson(mergerAction)}");
            var squareSourceGameObjectsList = FindAllSquareGameObjectsSameValueActive(mergerAction);
            var squareTargetGameObject = FindSquareGameObjectActiveByIndex(mergerAction.squareTarget.index);

            foreach (var squareSourceGameObject in squareSourceGameObjectsList)
            {
                sequence.Join(squareSourceGameObject.transform
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
    }

    private List<Square> FindAllSquareGameObjectsSameValueActive(MergerAction mergerAction)
    {
        return _squaresList.FindAll(squareGameObj =>
            squareGameObj.gameObject.activeSelf &&
            IsCompareSquareSameIndex(mergerAction.squareSources, squareGameObj));
    }

    private Square FindSquareGameObjectActiveByIndex(int index)
    {
        return _squaresList.Find(squareGameObj =>
            squareGameObj.squareData.index == index && squareGameObj.gameObject.activeSelf);
    }

    private void ChangeAction(Sequence sequence)
    {
        sequence.Append(nextSquare.transform.DOMove(nextSquare.transform.position, 0));
    }

    private bool IsCompareSquareSameIndex(List<SquareData> squareSources, Square squareGameObj)
    {
        var isSquareSamePosition = squareSources.Any(squareData => squareData.index == squareGameObj.squareData.index);
        return isSquareSamePosition;
    }

    private void SortUI(Sequence sequence, List<MergerAction> mergerActionList)
    {
        ChangeAction(sequence);
        foreach (var mergerAction in mergerActionList)
        {
            // Debug.Log($"Sort {JsonUtility.ToJson(mergerAction)}");
            //
            // Debug.Log($"mergerAction.squareSources[0].index {mergerAction.squareSources[0].index}");
            
            var squareSourceGameObject = FindSquareGameObjectActiveByIndex(mergerAction.squareSources[0].index);
            // Debug.Log($"mergerAction.squareTarget.Position.y {mergerAction.squareTarget.Position.y}");
            
            sequence.Append(squareSourceGameObject.transform
                .DOMoveY(mergerAction.squareTarget.Position.y, MERGE_DURATION)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    squareSourceGameObject.SetValue(mergerAction.newSquareValue);
                    squareSourceGameObject.SetIndex(mergerAction.squareTarget.index);
                    // Debug.Log($"squareSourceGameObject2.index {squareSourceGameObject.squareData.index}");
                })
            );
        }
    }

    private float GetDurationMove(Vector2 posSource, Vector2 posTarget)
    {
        var distance = Vector2.Distance(posSource, posTarget);
        return (distance / 4 + 3) / 10;
    }

    private void ProcessingData(int column)
    {
        int countActionsList;
        Shoot(column);

        do
        {
            countActionsList = _actionsWrapList.Count;

            MergeAllBlock();
            if (countActionsList == _actionsWrapList.Count)
            {
                break;
            }

            SortAllBlock();
        } while (countActionsList < _actionsWrapList.Count);
    }

    private void Shoot(int column)
    {
        _actionsList = new();
        var action = new MergerAction();
        var squareTarget = GetEmptySquareDataTargetByColumn(column);
        var squareSource = new SquareData(
            new Utils.Cell(column, boardRow),
            boardRow * boardCol + column,
            _newSquareValue
        );

        action.squareTarget = squareTarget;
        action.squareSources.Add(squareSource);
        action.newSquareValue = _newSquareValue;

        _actionsList.Add(action);

        _actionsWrapList.Add(new MergerActionWrap(_actionsList, ActionType.Shoot));

        squareTarget.value = _newSquareValue;
        _processingSquare = squareTarget;
    }

    private SquareData GetEmptySquareDataTargetByColumn(int column)
    {
        return squareDatas.Find(block => block.value == 0 && block.cell.Column == column);
    }

    private void ResetBoard()
    {
        for (var y = boardRow; y > 0; y--)
        {
            for (var x = 0; x < boardCol; x++)
            {
                squareDatas.Add(new SquareData(
                    new Utils.Cell(x, boardRow - y),
                    x + (boardRow - y) * boardCol,
                    0));
            }
        }
    }

    private void RenderLineColumn()
    {
        for (int i = 0; i < boardCol; i++)
        {
            var posLine = new Vector2(i * 2 - boardRow, 0);
            var line = Instantiate(lineColumn, posLine, Quaternion.identity, lineParentTransform);
            line.GetComponent<LineColumn>().Column = i;
            _lineColumnList.Add(line);
        }
    }

    private void MergeAllBlock()
    {
        _actionsList = new();
        var squareMergeOrderByCountSameValueList = squareDatas
            .Where(block => block.value > 0)
            .Select(block => new
            {
                block,
                countSquareSameValue = squareDatas.Count(squareData => IsBlockCanMerge(squareData, block))
            })
            .Where(data => data.countSquareSameValue > 0)
            .OrderByDescending(data => data.countSquareSameValue);

        foreach (var data in squareMergeOrderByCountSameValueList)
        {
            if (data.countSquareSameValue == 1)
            {
                MergeBlock(data.block);
            }
            else
            {
                MergeMultiBlock(data.block);
            }
        }

        if (!_actionsList.Any())
        {
            return;
        }
        _actionsWrapList.Add(new MergerActionWrap(_actionsList, ActionType.MergeAllBlock));
    }

    private bool IsBlockCanMerge(SquareData squareData, SquareData block)
    {
        var isHasValue = squareData.value > 0;
        var isSameValue = block.value == squareData.value;
        var isSquareRight = squareData.cell.Column == block.cell.Column + 1 && squareData.cell.Row == block.cell.Row;
        var isSquareLeft = squareData.cell.Column == block.cell.Column - 1 && squareData.cell.Row == block.cell.Row;
        var isSquareDown = squareData.cell.Row == block.cell.Row + 1 && squareData.cell.Column == block.cell.Column;
        var isSquareUp = squareData.cell.Row == block.cell.Row - 1 && squareData.cell.Column == block.cell.Column;
        return isHasValue && isSameValue && (isSquareRight || isSquareLeft || isSquareDown || isSquareUp);
    }

    private void MergeBlock(SquareData block)
    {
        SquareData squareSource;
        SquareData squareTarget;
        var action = new MergerAction();

        SquareData squareDataSameValue = squareDatas.Find(squareData => IsBlockCanMerge(squareData, block));

        if (squareDataSameValue is null)
        {
            return;
        }

        var newValue = block.value * 2;
        var isSameColumn = squareDataSameValue.cell.Row == block.cell.Row - 1;
        var isSameRowRight = block.cell.Column > squareDataSameValue.cell.Column &&
                             squareDataSameValue.cell.Column >= _processingSquare.cell.Column;
        var isSameRowLeft = block.cell.Column < squareDataSameValue.cell.Column &&
                            squareDataSameValue.cell.Column <= _processingSquare.cell.Column;

        if (isSameColumn || isSameRowRight || isSameRowLeft)
        {
            squareSource = block;
            squareTarget = squareDataSameValue;
        }
        else
        {
            squareSource = squareDataSameValue;
            squareTarget = block;
        }

        action.squareSources.Add(new SquareData(squareSource.cell, squareSource.index, squareSource.value));
        action.squareTarget = new SquareData(squareTarget.cell, squareTarget.index, squareTarget.value);
        action.newSquareValue = newValue;

        _actionsList.Add(action);
        squareTarget.value = newValue;
        squareSource.value = 0;
        if (squareSource == _processingSquare)
        {
            _processingSquare = squareTarget;
        }
    }

    private void MergeMultiBlock(SquareData cellCheck)
    {
        var action = new MergerAction();
        var countBlockSameValue = 0;

        var squareLeft = GetSquareDataByCell(new Utils.Cell(cellCheck.cell.Column - 1, cellCheck.cell.Row));
        var squareUp = GetSquareDataByCell(new Utils.Cell(cellCheck.cell.Column, cellCheck.cell.Row - 1));
        var squareRight = GetSquareDataByCell(new Utils.Cell(cellCheck.cell.Column + 1, cellCheck.cell.Row));
        var squareDown = GetSquareDataByCell(new Utils.Cell(cellCheck.cell.Column, cellCheck.cell.Row + 1));

        countBlockSameValue = CountBlockSameValue(squareLeft, cellCheck, countBlockSameValue, action);
        countBlockSameValue = CountBlockSameValue(squareUp, cellCheck, countBlockSameValue, action);
        countBlockSameValue = CountBlockSameValue(squareRight, cellCheck, countBlockSameValue, action);
        countBlockSameValue = CountBlockSameValue(squareDown, cellCheck, countBlockSameValue, action);

        if (countBlockSameValue == 0)
        {
            return;
        }

        var newValue = cellCheck.value * (int)Mathf.Pow(2, countBlockSameValue);

        action.squareTarget = new SquareData(cellCheck.cell, cellCheck.index, cellCheck.value);
        action.newSquareValue = newValue;

        _actionsList.Add(action);

        cellCheck.value = newValue;
    }

    private SquareData GetSquareDataByCell(Utils.Cell cell)
    {
        return squareDatas.Find(item => item.cell.Row == cell.Row && item.cell.Column == cell.Column);
    }

    private int CountBlockSameValue(SquareData squareData, SquareData cellCheck, int countBlockSameValue,
        MergerAction action)
    {
        if (squareData != null && squareData.value == cellCheck.value && squareData.value > 0)
        {
            action.squareSources.Add(new SquareData(squareData.cell, squareData.index, squareData.value));
            squareData.value = 0;
            countBlockSameValue++;
            if (squareData == _processingSquare)
            {
                _processingSquare = cellCheck;
            }
        }

        return countBlockSameValue;
    }

    private void SortAllBlock()
    {
        _actionsList = new();
        var emptyBlocksUpRowList = squareDatas
            .FindAll(item => item.value == 0 &&
                             squareDatas.Any(squareDownRow =>
                                 squareDownRow.index == item.index + boardCol &&
                                 squareDownRow.value > 0));

        if (!emptyBlocksUpRowList.Any())
        {
            return;
        }

        foreach (var emptyBlocksUpRow in emptyBlocksUpRowList)
        {
            var squareHasValueDownRowList = GetSquaresDataHasValueDownRowByCell(emptyBlocksUpRow);

            foreach (var squareHasValueDownRow in squareHasValueDownRowList)
            {
                SortBlock(squareHasValueDownRow.cell,
                    new Utils.Cell(squareHasValueDownRow.cell.Column, squareHasValueDownRow.cell.Row - 1));
            }
        }

        if (!_actionsList.Any())
        {
            return;
        }

        _actionsWrapList.Add(new MergerActionWrap(_actionsList, ActionType.SortAllBlock));
    }

    private void SortBlock(Utils.Cell cellCheck, Utils.Cell cellUp)
    {
        var action = new MergerAction();
        var squareUp = GetSquareDataByCell(cellUp);
        var squareCheck = GetSquareDataByCell(cellCheck);

        action.squareSources.Add(new SquareData(squareCheck.cell, squareCheck.index, squareCheck.value));
        action.squareTarget = new SquareData(squareUp.cell, squareUp.index, squareUp.value);
        action.newSquareValue = squareCheck.value;

        _actionsList.Add(action);

        squareUp.value = squareCheck.value;
        squareCheck.value = 0;

        if (squareCheck == _processingSquare)
        {
            _processingSquare = squareUp;
        }
    }

    private IEnumerable<SquareData> GetSquaresDataHasValueDownRowByCell(SquareData squareEmptyUpRow)
    {
        return squareDatas.Where(squareData => squareData.cell.Column == squareEmptyUpRow.cell.Column &&
                                               squareData.value > 0 &&
                                               squareData.cell.Row > squareEmptyUpRow.cell.Row)
                .OrderBy(squareData => squareData.index)
            ;
    }

    private void PrinterSquaresData()
    {
        print("__________");
        squareDatas
            .FindAll(e => e.value > 0)
            .ForEach(e => { Debug.Log(JsonUtility.ToJson(e)); });
    }

    private void SetRandomSquareValue()
    {
        _randomNum = Random.Range(0, _squareValueList.Count);
        _newSquareValue = _squareValueList[_randomNum];
        nextSquare.SetValue(_newSquareValue);
    }

    #endregion
}