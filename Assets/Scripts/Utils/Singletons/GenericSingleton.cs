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
                    Debug.LogWarning($"Singleton<{typeof(T)}>: Instance not found in the scene.");
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
            
            // TODO: temp since we will never have to destory gameobject
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Teleport existing instance to this objects position
            instance.transform.position = transform.position;
            Destroy(gameObject);
        }
    }
}