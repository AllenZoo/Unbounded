using Sirenix.OdinInspector;
using UnityEngine;

public class ContextInitializer<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField, Required] protected BaseContext contextRef;
    [SerializeField, Required] protected T context;
    [SerializeField] protected bool loadOnEnable = true;


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


