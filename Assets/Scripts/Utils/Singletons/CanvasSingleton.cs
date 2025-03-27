using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Singleton class is useful to make sure that we don't destory canvas object on load, and that we don't duplicate the objects.
/// </summary>
public class CanvasSingleton : Singleton<CanvasSingleton>
{
    private new void Awake()
    {
        base.Awake();
    }
}
