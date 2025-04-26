using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InitializerUtil
{
    public static T FindComponentInParent<T>(GameObject obj) where T : Component
    {
        var component = obj.GetComponentInParent<T>();
        if (component == null)
        {
            Debug.LogError($"{typeof(T).Name} not found in parent for object [{obj}] with root object [{obj.transform.root.name}].");
        }
        return component;
    }
}
