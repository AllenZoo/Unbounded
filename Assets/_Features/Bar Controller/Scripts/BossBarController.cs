using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BossBarController : BarController
{
    [SerializeField, Required] public TextMeshProUGUI bossText;

    private BossBarConfig bossConfig;

    public void Setup(BossBarConfig config)
    {
        bossConfig = config;
        Render(); // Show boss name immediately
    }

    protected override void Render()
    {
        base.Render();
        if (!bossText) return;

        bossConfig = barContext.BossBarConfig;

        if (bossText != null && bossConfig != null)
        {
            bossText.text = bossConfig.BossName;
        }
    }
}


