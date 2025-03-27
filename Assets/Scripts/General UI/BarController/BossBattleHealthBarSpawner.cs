using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// Class that spawns (or sets active) a boss health bar when the boss is aggroed to player.
// TODO: REFACTOR
public class BossBattleHealthBarSpawner : MonoBehaviour
{
    [SerializeField, Required] private LocalEventHandler localEventHandler;
    [SerializeField, Required] private BarContext barContext;

    private void Awake()
    {
        Assert.IsNotNull(localEventHandler);
        Assert.IsNotNull(barContext);

        if (localEventHandler == null)
        {
            localEventHandler = GetComponentInParent<LocalEventHandler>();
            if (localEventHandler == null)
            {
                Debug.LogError("LocalEventHandler unassigned and not found in parent for object [" + gameObject +
                                        "] with root object [" + gameObject.transform.root.name + "] for BossBattleHealthBarSpawner.cs");
            }
        }
    }

    private void Start()
    {
        LocalEventBinding<OnAggroStatusChangeEvent> aggroChangeBinding = new LocalEventBinding<OnAggroStatusChangeEvent>(OnAggroStatusChange);
        localEventHandler.Register(aggroChangeBinding);
    }

    private void OnAggroStatusChange(OnAggroStatusChangeEvent e)
    {
        barContext.IsVisible = e.isAggroed;
        barContext.LEH = localEventHandler;
    }
}
