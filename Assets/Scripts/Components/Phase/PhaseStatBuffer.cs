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
    [SerializeField] private LocalEventHandler localEventHandler;

    [SerializeField] private PhaseManager phaseManager;

    [SerializedDictionary (keyName: "Phase #", valueName: "Buffs")]
    [SerializeField] private SerializedDictionary<int, List<StatModifierContainer>> phaseBuffsMapper;

    // TODO: eventually remove after refactoring StatComponent OnStatChange event.
    [SerializeField] private StatComponent stats;

    [SerializeField] private float hpThresholdForRagePhase = 100f;

    private void Awake()
    {
        Assert.IsNotNull(phaseManager, "Phase Stat Buffer requires Phase Manager");

        if (localEventHandler == null)
        {
            localEventHandler = GetComponentInParent<LocalEventHandler>();
            if (localEventHandler == null)
            {
                Debug.LogError("LocalEventHandler unassigned and not found in parent for object [" + gameObject +
                                       "] with root object [" + gameObject.transform.root.name + "] for PhaseStatBuffer.cs");
            }
        }
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
        if (sc.health <= hpThresholdForRagePhase)
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

        foreach (IStatModifierContainer buff in phaseBuffsMapper[phase])
        {
            StatModifier statModifier = buff.GetModifier();
            localEventHandler.Call(new AddStatModifierRequest { statModifier = statModifier });
        }
    }

    private void DeApplyPhaseBuff(int phase)
    {
        if (!phaseBuffsMapper.ContainsKey(phase))
        {
            // No buffs for this phase, thus nothing to remove
            return;
        }

        foreach (IStatModifierContainer buff in phaseBuffsMapper[phase])
        {
            StatModifier statModifier = buff.GetModifier();
            statModifier.Dispose();
        }
    }
}
