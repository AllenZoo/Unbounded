using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For storing static base stat data.
/// </summary>
[CreateAssetMenu(menuName = "ScriptableObjs/Stat Container", fileName ="new stat container")]
public class SO_StatContainer : ScriptableObject
{
    public float health;
    public float maxHealth;
    public float attack;
    public float defense;
    public float dexterity;
    public float speed;
    public float gold;
}
