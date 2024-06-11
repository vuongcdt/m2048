using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance => _instance;

    private void Awake()
    {
        if (!_instance)
        {
            _instance = this.GetComponent<T>();
            return;
        }

        if (this.gameObject.GetInstanceID() != _instance.GetInstanceID())
        {
            Destroy(this.gameObject);
        }
    }
}