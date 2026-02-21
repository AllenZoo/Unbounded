using System;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class BaseContext : ScriptableObject {
    public abstract void ResetState();
}
public class BaseContext<T> : BaseContext where T : MonoBehaviour
{
    public Action<T> OnContextChanged;
    [SerializeField, ReadOnly] private T context;
    private bool initialized = false;

    public void Init(T context)
    {
        // if context != null -> already intialized.
        if (initialized && this.context != null)
        {
            Debug.LogWarning("Trying to initialize a context that has already been initialized");
            return;
        }

        if (context == null) {
            Debug.LogWarning("Context being intialized is null!");
            return;
        }


        initialized = true;
        this.context = context;
    }

    /// <summary>
    /// Returns context if initialized. Otherwise, returns null.
    /// </summary>
    /// <returns></returns>
    public Optional<T> GetContext()
    {
        if (!initialized)
        {
            return new Optional<T>(null);
        }

        return new Optional<T>(context);
    }

    public void SetContext(T context)
    {
        this.context = context;
        OnContextChanged?.Invoke(context);
    }


    public override void ResetState()
    {
        initialized = false;
        context = null;
    }

//#if UNITY_EDITOR || DEVELOPMENT_BUILD
//    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
//    private static void ResetAll()
//    {
//        var all = Resources.FindObjectsOfTypeAll<BaseContext>();

//        foreach (var context in all)
//        {
//            context.ResetState();
//        }
//    }
//#endif
}