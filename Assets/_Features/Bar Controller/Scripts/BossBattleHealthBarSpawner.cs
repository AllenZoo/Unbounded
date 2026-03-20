using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// Class that spawns (or sets active) a boss health bar when the boss is aggroed to player.
public class BossBattleHealthBarSpawner : MonoBehaviour
{
    [SerializeField, Required] private LocalEventHandler leh;
    [SerializeField, Required] private BarChannel barChannel;
    [SerializeField, Required] private BossBarConfig barConfig;
    [SerializeField, Required] private StatComponent stat;


    private LocalEventBinding<OnAggroStatusChangeEvent> aggroChangeBinding;
    private void Awake()
    {
        Assert.IsNotNull(leh);
        Assert.IsNotNull(barChannel);

        if (leh == null) leh = InitializerUtil.FindComponentInParent<LocalEventHandler>(this.gameObject);

        aggroChangeBinding = new LocalEventBinding<OnAggroStatusChangeEvent>(OnAggroStatusChange);
    }

    private void OnEnable()
    {
        leh.Register(aggroChangeBinding);
    }
    private void OnDisable()
    {
        leh.Unregister(aggroChangeBinding);
    }


    private void OnAggroStatusChange(OnAggroStatusChangeEvent e)
    {
        barChannel.IsVisible = e.isAggroed;
        barChannel.BossBarConfig = barConfig;
        barChannel.LEH = leh;
        barChannel.Stat = stat;
    }
}
