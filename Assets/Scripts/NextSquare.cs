using System;
using UnityEngine;

public class NextSquare:MonoBehaviour
{
    [SerializeField] private Square square;
    private void FixedUpdate()
    {
        // square.value = BoardManager.Instance.SquareNextValue;
    }
}