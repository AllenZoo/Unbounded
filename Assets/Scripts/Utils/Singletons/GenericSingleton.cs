using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : SerializedMonoBehaviour where T : SerializedMonoBehaviour
{
    protected static T instance;

    // Access the singleton instance through this property
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<T>();

                if (instance == null)
                {
                    Debug.LogWarning($"Singleton<{typeof(T)}>: Instance not found in the scene. Was PersistantGameplay unloaded? (Make sure it is not active scene)");
                }
            }

            return instance;
        }
    }

    // Optional Awake method to initialize the singleton
    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
}