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
 
    // Potential Idea:
    // Store list of PhaseTransition objects that have a certain condition to be fulfilled, and also a priority to determine which one to transition
    // to if multiple conditions are fulfilled.

    // Current rotation of phases that the boss is in. Will rotate randomly after a period of time.
    [SerializeField] private List<int> rotatedPhaseList = new List<int>();

    [Tooltip("Time between phase rotations in seconds. Item1 = min, Item2 = max")]
    [SerializeField] private SerializableTuple<int, int> rotationTimeRange = new SerializableTuple<int, int>(5, 10);

    [Tooltip("Phase that the boss enters when it's ANGRY. Generally this happens when hp is low but could be triggered due" +
        "to different reasons..")]
    [SerializeField] private int ragePhase = 3;
    private float rotationTimer = 0f;
    private bool shouldRotate = true;

    private void Update()
    {
        rotationTimer -= Time.deltaTime;
        if (shouldRotate && rotationTimer <= 0f)
        {
            RotatePhase();
            rotationTimer = UnityEngine.Random.Range(rotationTimeRange.Item1, rotationTimeRange.Item2);
        }
    }

    public int Phase { get { return phase; } private set { phase = value; } }

    public void ChangePhase(int newPhase)
    {
        int oldPhase = phase;
        phase = newPhase;
        OnPhaseChange?.Invoke(oldPhase, phase);
    }

    public void TriggerRagePhase()
    {
        // Debug.Log("Triggered rage phase!");
        // Disable random phase rotation when in rage phase.
        shouldRotate = false;
        ChangePhase(ragePhase);
    }

    // Rotate the phase list randomly.
    private void RotatePhase()
    {
        int phases = rotatedPhaseList.Count;
        if (phases <= 1)
        {
            return;
        }

        int randomIndex = UnityEngine.Random.Range(0, phases);
        int newPhase = rotatedPhaseList[randomIndex];
        ChangePhase(newPhase);
    }
}


[Serializable]
public class SerializableTuple<T1, T2>
{
    public T1 Item1;
    public T2 Item2;

    public SerializableTuple(T1 item1, T2 item2)
    {
        Item1 = item1;
        Item2 = item2;
    }
}