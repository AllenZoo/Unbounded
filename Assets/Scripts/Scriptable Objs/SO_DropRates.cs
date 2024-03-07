using System.Collections;
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
    public float emptyDropRate;
    public SO_DropRates data;
}

[System.Serializable]
public class DropRate
{
    public Item item;

    // Ranges between: [0, 1]
    public float rate;
}
