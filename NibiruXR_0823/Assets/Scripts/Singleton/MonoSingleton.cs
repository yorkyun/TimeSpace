using UnityEngine;

/// <summary>
/// 需要使用Unity生命周期的单例模式
/// </summary>
public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    protected static T instance = null;

    public static T Instance()
    {
        if (instance == null)
        {
            T[] objects = FindObjectsOfType<T>();
            if (objects.Length == 1)
            {
                instance = objects[0];
                DontDestroyOnLoad(instance.gameObject);
            }
            else if (objects.Length > 1)
            {
                Debug.Log("More than 1!");
                return instance;
            }

            if (instance == null)
            {
                string instanceName = typeof(T).Name;
                Debug.Log("Instance Name: " + instanceName);
                GameObject instanceGO = GameObject.Find(instanceName);

                if (instanceGO == null)
                    instanceGO = new GameObject(instanceName);
                instance = instanceGO.AddComponent<T>();
                DontDestroyOnLoad(instanceGO);  // 不会被释放
                Debug.Log("Add New Singleton " + instance.name + " in Game!");
            }
            else
            {
                Debug.Log("Already exist: " + instance.name);
            }
        }

        return instance;
    }

    protected virtual void OnDestroy()
    {
        instance = null;
    }
}
