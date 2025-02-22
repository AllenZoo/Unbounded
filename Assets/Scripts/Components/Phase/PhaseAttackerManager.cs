using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// Maps SO_Attacker to certain phases and sets them when that phase is active.
public class PhaseAttackerManager : MonoBehaviour
{
    [SerializeField] private PhaseManager phaseManager;

    [SerializedDictionary("Phase", "SO_Attacker Data of Phase")]
    [SerializeField] private SerializedDictionary<int, Attacker> attackerMap;

    [SerializeField] private Attacker defaultAttacker;

    [SerializeField] private Attacker attacker;

    private void Awake()
    {
        Assert.IsNotNull(phaseManager, "Phase manager is required for this component!");
        Assert.IsNotNull(defaultAttacker, "Default attack mode is required for this component!");
        Assert.IsNotNull(attacker, "Attacker is required for this component!");
    }

    private void Start()
    {
        phaseManager.OnPhaseChange += OnPhaseChange;
        ChangeAttackerData();
    }

    private void OnPhaseChange(int oldPhase, int phase)
    {
        if (oldPhase == phase)
        {
            return;
        }

        ChangeAttackerData();
    }

    private void ChangeAttackerData()
    {
        if (!attackerMap.ContainsKey(phaseManager.Phase))
        {
            // TODO: rethink whole logic 
            // No active attack mode for this phase. Use default.
            //attacker.SetAttackerData(defaultAttackMode);
            return;
        }

        //attacker.SetAttackerData(attackerMap[phaseManager.Phase]);
    }

}
