using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// ContextInitializer is the MonoBehaviour component responsible for initializing a BaseContext with the correct reference.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ContextInitializer<T> : MonoBehaviour where T : Component
{
    [SerializeField, Required] protected BaseContext contextRef;
    [SerializeField, Required] protected T context;
    [SerializeField] protected bool loadOnAwake = true;
    [SerializeField] protected bool loadOnEnable = true;

    protected void Awake()
    {
        if (loadOnAwake)
        {
            LoadContext();
        }
    }

    protected void OnEnable()
    {
        if (loadOnEnable)
        {
            LoadContext();
        }
    }

    public void LoadContext()
    {
        if (contextRef is BaseContext<T> typedContext)
        {
            typedContext.Init(context);
        }
        else
        {
            Debug.LogError($"Context ref is not of expected type BaseContext<{typeof(T).Name}>");
        }
    }
}


