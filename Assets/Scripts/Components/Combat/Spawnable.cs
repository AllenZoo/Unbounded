using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnable : MonoBehaviour
{
    /// <summary>
    /// LocalEventHandler for the spawnable.
    /// </summary>
    [SerializeField] private LocalEventHandler localEventHandler;

    /// <summary>
    /// LocalEventHandler for the spawner.
    /// </summary>
    private LocalEventHandler spawnerLocalEventHandler;

    private void Awake()
    {
        if (localEventHandler == null)
        {
            localEventHandler = GetComponentInParent<LocalEventHandler>();
            if (localEventHandler == null)
            {
                Debug.LogError("LocalEventHandler unassigned and not found in parent for object [" + gameObject +
                                        "] with root object [" + gameObject.transform.root.name + "] for Spawnable.cs");
            }
        }
    }

    private void Start()
    {
        LocalEventBinding<OnDeathEvent> onDeathBinding = new LocalEventBinding<OnDeathEvent>(TriggerOnDespawn);
        localEventHandler.Register(onDeathBinding);
    }

    public void SetSpawnerLocalEventHandler(LocalEventHandler localEventHandler)
    {
        this.spawnerLocalEventHandler = localEventHandler;
    }

    public void TriggerOnSpawn()
    {
        if (spawnerLocalEventHandler != null)
        {
            spawnerLocalEventHandler.Call(new OnSpawnEvent { spawn = this });
        }
    }

    public void TriggerOnDespawn(OnDeathEvent e)
    {
        // Checks to see if spawnable was spawned in or naturally there.
        if (spawnerLocalEventHandler != null)
        {
            spawnerLocalEventHandler.Call(new OnDespawnEvent { spawn = this });
        }
       
    }
}
