using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NumericPlayerStatDisplayer : MonoBehaviour
{
    [Required][SerializeField] Stat statToDisplay;

    [Tooltip("the text reference we update the text on")]
    [Required][SerializeField] TextMeshProUGUI text;

    private StatComponent playerStat;

    private void Start()
    {
        playerStat = PlayerSingleton.Instance.GetPlayerStatComponent();
        playerStat.OnStatChange += Render;

        Render();
    }

    private void Render()
    {
        float statValue = playerStat.GetStatValue(statToDisplay);
        text.text = "" + statValue;
    }
}
