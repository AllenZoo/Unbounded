using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Drop Rates", menuName = "ScriptableObjs/Drop Rates")]
public class SO_DropRates : ScriptableObject
{
    public List<DropRate> dropRates = new List<DropRate>();
}

[System.Serializable]
public class DropRates
{
    [Tooltip("The rate at which the loot bag will drop nothing. Typical scale of item drop rates: [0 - 1].")]
    public float emptyDropRate = 0.5f;
    public SO_DropRates data;
}

[System.Serializable]
public class DropRate
{
    public Item item;

    // Ranges between: [0, 1]
    [Tooltip("Ranges between: [0, 1]")]
    public float rate;
}
