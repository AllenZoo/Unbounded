using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For entities that have phases. Determines when to change phases.
public class PhaseManager : MonoBehaviour
{
    // Old Phase, New Phase
    public event Action<int, int> OnPhaseChange;
    [SerializeField] private int phase = 0;
    
    public int Phase { get { return phase; } private set { phase = value; } }


    public void ChangePhase(int newPhase)
    {
        int oldPhase = phase;
        phase = newPhase;
        OnPhaseChange?.Invoke(oldPhase, phase);
    }
}
