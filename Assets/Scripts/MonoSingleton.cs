using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static readonly object locker = new object();
    private static T instance = null;

    public static T Instance
    {
        get
        {
            lock (locker)
            {
                if (instance == null)
                {
                    instance = FindObjectOfType(typeof(T)) as T;

                    // Create new instance if one doesn't already exist.
                    if (instance == null)
                    {
                        // Need to create a new GameObject to attach the singleton to.
                        var createdObj = new GameObject();
                        instance = createdObj.AddComponent<T>();
                        createdObj.name = typeof(T).ToString();
                    }
                }

                return instance;
            }
        }
    }
}
