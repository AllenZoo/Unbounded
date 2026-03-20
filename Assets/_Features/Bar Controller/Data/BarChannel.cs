using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Model for the bar UI. Acts as a channel between an entity (player/boss) and the UI.
/// </summary>
[CreateAssetMenu(fileName ="new Bar Channel", menuName ="System/General UI/BarContext")]
public class BarChannel : ScriptableObject, IModel
{
    public Action OnBarContextChange;

    public LocalEventHandler LEH { get { return leh; } set { leh = value; OnBarContextChange?.Invoke(); } }
    [SerializeField, ReadOnly] private LocalEventHandler leh;

    public bool IsVisible { get { return isVisible; } set { isVisible = value; OnBarContextChange?.Invoke(); } }
    [SerializeField, ReadOnly] private bool isVisible;

    public BossBarConfig BossBarConfig { get { return bossBarConfig; } set { bossBarConfig = value; OnBarContextChange?.Invoke(); } }
    [SerializeField, ReadOnly] private BossBarConfig bossBarConfig;

    public StatComponent Stat { get { return stat; } set { stat = value; OnBarContextChange?.Invoke(); } }
    [SerializeField, ReadOnly] private StatComponent stat;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void ResetAllChannels()
    {
        var channels = Resources.FindObjectsOfTypeAll<BarChannel>();
        foreach (var channel in channels)
        {
            channel.Clear();
        }
    }

    public void Clear()
    {
        leh = null;
        stat = null;
        bossBarConfig = null;
        isVisible = false;
        OnBarContextChange?.Invoke();
    }
}
