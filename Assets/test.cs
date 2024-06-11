using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.DOMoveX(8, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            Debug.Log("123");
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
