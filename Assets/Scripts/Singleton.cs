using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance => _instance;

    private void Awake()
    {
        if (!_instance)
        {
            _instance = GetComponent<T>();
            return;
        }

        if (gameObject.GetInstanceID() != _instance.GetInstanceID())
        {
            Destroy(gameObject);
        }

        AwakeCustom();
    }

    protected virtual void AwakeCustom()
    {
    }
}