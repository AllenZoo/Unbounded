using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// Class that spawns (or sets active) a boss health bar when the boss is aggroed to player.
public class BossBattleHealthBarSpawner : MonoBehaviour
{
    [SerializeField] private AggroPossessor aggro;
    [SerializeField] private StatComponent stats;
    private BarController bossHealthBarController;
    private void Awake()
    {
        Assert.IsNotNull(aggro);
        Assert.IsNotNull(stats);
    }

    private void Start()
    {
        aggro.OnAggroStatusChange += OnAggroStatusChange;

        bossHealthBarController = CanvasSingleton.Instance.bossBarController;
    }

    private void OnAggroStatusChange(bool isAggroed)
    {
        bossHealthBarController.gameObject.SetActive(isAggroed);

        if (bossHealthBarController.gameObject.activeSelf)
        {
            bossHealthBarController.Set(stats, Stat.HP);
        }
    }
}
