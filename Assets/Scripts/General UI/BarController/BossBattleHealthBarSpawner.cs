using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// Class that spawns (or sets active) a boss health bar when the boss is aggroed to player.
public class BossBattleHealthBarSpawner : MonoBehaviour
{
    [SerializeField, Required] private LocalEventHandler localEventHandler;
    [SerializeField, Required] private BarContext barContext;

    private void Awake()
    {
        Assert.IsNotNull(localEventHandler);
        Assert.IsNotNull(barContext);

        localEventHandler = InitializerUtil.FindComponentInParent<LocalEventHandler>(this.gameObject);
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
