using UnityEngine;

public class MonoCache : MonoBehaviour
{
    // private StoreManager _storeManager;
    internal BoardManager boardManager;
    internal GameObject gameObjectCache;
    internal Transform transformCache;
    private void Awake()
    {
        boardManager = BoardManager.Instance;
        gameObjectCache = gameObject;
        transformCache = transform;
        AwakeCustom();
    }

    protected virtual void AwakeCustom()
    {
        
    }
}