using System;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Base Context acts as a link between scene and system using scriptable objects.
/// It allows systems to have a reference to scene objects without directly referencing them, thus decoupling the system from the scene.
/// 
/// Base Context defines type of Monobehaviour to store
/// Base Context Initializer is responsible for initializing the context with the correct reference. 
///     It should be placed on a scene object and reference the context to initialize, as well as the scene object to pull the reference from.
///     
/// 
/// New Scriptable Object Creation Paths should be: "System/Contexts/[type of context]"
/// Sample: [CreateAssetMenu(fileName = "new damageable context", menuName = "System/Contexts/DamageableContext")]
/// </summary>

public abstract class BaseContext : ScriptableObject {
    public abstract void ResetState();

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetAllContexts()
    {
        // FindObjectsOfTypeAll will find the assets even if not currently in the scene
        var all = Resources.FindObjectsOfTypeAll<BaseContext>();
        foreach (var ctx in all)
        {
            ctx.ResetState();
        }
    }
#endif

}
public class BaseContext<T> : BaseContext where T : Component
{
    public Action<T> OnContextChanged;

    // In the future, we can consider having this as a list, so that we can store multiple monobehaviours/components that share the same context scriptable object.
    // Currently the Context Initializer that lasts fires will set the context.
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
        OnContextChanged?.Invoke(context);
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
        OnContextChanged = null;
    }

}