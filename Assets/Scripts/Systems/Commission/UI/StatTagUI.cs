using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Class that handles properly displaying a <Stat, Int> pair UI.
/// </summary>
public class StatTagUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statTagText;

    private Tuple<Stat, int> stat;

    public void SetStat(Tuple<Stat, int> stat)
    {
        this.stat = stat;
        Render();
    }

    private void Render()
    {
        statTagText.text = stat.Item1.ToString() + ": " + stat.Item2;
    }
}
