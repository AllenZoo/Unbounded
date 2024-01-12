using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CanvasSingleton : Singleton<CanvasSingleton>
{
    [Tooltip("For displaying health bar of bosses during fights.")]
    public BarController bossBarController;

    private new void Awake()
    {
        base.Awake();
        Assert.IsNotNull(bossBarController);
    }
}
