using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// Class that spawns (or sets active) a boss health bar when the boss is aggroed to player.
public class BossBattleHealthBarSpawner : MonoBehaviour
{
    [SerializeField, Required] private LocalEventHandler leh;
    [SerializeField, Required] private BarContext barContext;
    [SerializeField, Required] private BossBarConfig barConfig;

    private void Awake()
    {
        Assert.IsNotNull(leh);
        Assert.IsNotNull(barContext);

        if (leh == null) leh = InitializerUtil.FindComponentInParent<LocalEventHandler>(this.gameObject);
    }

    private void Start()
    {
        LocalEventBinding<OnAggroStatusChangeEvent> aggroChangeBinding = new LocalEventBinding<OnAggroStatusChangeEvent>(OnAggroStatusChange);
        leh.Register(aggroChangeBinding);
    }

    private void OnAggroStatusChange(OnAggroStatusChangeEvent e)
    {
        barContext.IsVisible = e.isAggroed;
        barContext.BossBarConfig = barConfig;
        barContext.LEH = leh;
    }
}
