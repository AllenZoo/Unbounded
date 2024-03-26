using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnable : MonoBehaviour
{
    public event Action<Spawnable> OnSpawn;
    public event Action<Spawnable> OnDespawn;

    public void TriggerOnSpawn()
    {
        OnSpawn?.Invoke(this);
    }

    public void TriggerOnDespawn()
    {
        OnDespawn?.Invoke(this);
    }
}
