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
    [SerializeField] private SerializedDictionary<int, List<IStatModifier>> phaseBuffsMapper;

    [SerializeField] private StatComponent stats;

    private void Awake()
    {
        Assert.IsNotNull(phaseManager, "Phase Stat Buffer requires Phase Manager");

    }

    private void Start()
    {
        phaseManager.OnPhaseChange += OnPhaseChange;
        ApplyPhaseBuff(phaseManager.Phase);
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

    private void ApplyPhaseBuff(int phase)
    {
        if (!phaseBuffsMapper.ContainsKey(phase))
        {
            // No buffs for this phase
            return;
        }

        List<IStatModifier> buffs = phaseBuffsMapper[phase];
        foreach (IStatModifier buff in buffs)
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

        List<IStatModifier> buffs = phaseBuffsMapper[phase];
        foreach (IStatModifier buff in buffs)
        {
            stats.ModifyStat(new IStatModifier(buff.Stat, -buff.Value));
        }
    }
}
