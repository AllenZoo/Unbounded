using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// Class that spawns (or sets active) a boss health bar when the boss is aggroed to player.
public class BossBattleHealthBarSpawner : MonoBehaviour
{
    [SerializeField] private LocalEventHandler localEventHandler;
    [SerializeField] private StatComponent stats;
    private BarController bossHealthBarController;
    private void Awake()
    {
        Assert.IsNotNull(stats);

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
        bossHealthBarController = CanvasSingleton.Instance.bossBarController;

        LocalEventBinding<OnAggroStatusChangeEvent> aggroChangeBinding = new LocalEventBinding<OnAggroStatusChangeEvent>(OnAggroStatusChange);
        localEventHandler.Register(aggroChangeBinding);
    }

    private void OnAggroStatusChange(OnAggroStatusChangeEvent e)
    {
        bossHealthBarController.gameObject.SetActive(e.isAggroed);

        if (bossHealthBarController.gameObject.activeSelf)
        {
            bossHealthBarController.Set(localEventHandler, stats, BarTrackStat.HP);
        }
    }
}
