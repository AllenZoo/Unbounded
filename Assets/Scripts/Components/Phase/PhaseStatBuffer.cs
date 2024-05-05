using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

// For entities that have phases. Used to give certain buffs to entities based on their phase.
// Meant to be modified with inspector.
public class PhaseStatBuffer : MonoBehaviour
{
    [SerializeField] private PhaseManager phaseManager;

    [SerializedDictionary]
    [SerializeField] private SerializedDictionary<int, List<StatModifier>> phaseBuffsMapper;

    [SerializeField] private StatComponent stats;

    [SerializeField] private float hpThresholdForRagePhase = 100f;

    private void Awake()
    {
        Assert.IsNotNull(phaseManager, "Phase Stat Buffer requires Phase Manager");

    }

    private void Start()
    {
        phaseManager.OnPhaseChange += OnPhaseChange;
        ApplyPhaseBuff(phaseManager.Phase);

        stats.OnStatChange += OnStatChange;
    }

    private void OnPhaseChange(int oldPhase, int phase)
    {
        if (oldPhase == phase)
        {
            return;
        }

        // Remove old phase buffs
        DeApplyPhaseBuff(oldPhase);

        // Apply new phase buffs
        ApplyPhaseBuff(phase);
    }

    // TODO: handle this logic somewhere else?
    private void OnStatChange(StatComponent sc, StatModifier stat)
    {
        // Debug.Log("Cur health: " + sc.GetCurStat(Stat.HP) + " max health: " + sc.GetMaxStat(Stat.HP));
        if (sc.GetCurStat(Stat.HP) <= hpThresholdForRagePhase)
        {
            // Debug.Log("Triggering rage phase");
            phaseManager.TriggerRagePhase();
        }
    }

    private void ApplyPhaseBuff(int phase)
    {
        if (!phaseBuffsMapper.ContainsKey(phase))
        {
            // No buffs for this phase
            return;
        }

        List<StatModifier> buffs = phaseBuffsMapper[phase];
        foreach (StatModifier buff in buffs)
        {
            stats.ModifyStat(buff);
        }
    }

    private void DeApplyPhaseBuff(int phase)
    {
        if (!phaseBuffsMapper.ContainsKey(phase))
        {
            // No buffs for this phase, thus nothing to remove
            return;
        }

        List<StatModifier> buffs = phaseBuffsMapper[phase];
        foreach (StatModifier buff in buffs)
        {
            stats.ModifyStat(new StatModifier(buff.Stat, -buff.Value));
        }
    }
}
