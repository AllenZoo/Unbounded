using System;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class BaseContext : ScriptableObject { }
public class BaseContext<T> : BaseContext where T : MonoBehaviour
{
    public Action<T> OnContextChanged;
    [SerializeField, ReadOnly] private T context;
    private bool initialized = false;

    public void Init(T context)
    {
        if (initialized)
        {
            Debug.LogWarning("Trying to initialize a context that has already been initialized");
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

    private void OnDisable()
    {
        ResetState();
    }

    private void OnDestroy()
    {
        ResetState();
    }

    private void ResetState()
    {
        initialized = false;
        context = null;
    }
}