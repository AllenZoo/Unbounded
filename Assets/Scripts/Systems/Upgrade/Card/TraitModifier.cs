using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TraitModifier : IUpgradeModifier
{
    [SerializeField] private bool addPiercing = false;
    [SerializeField] private int numAtksToAdd = 0;

    public void ApplyModifier()
    {
        throw new System.NotImplementedException();
    }
}
