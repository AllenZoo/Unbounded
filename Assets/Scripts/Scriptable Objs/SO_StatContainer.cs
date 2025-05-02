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
    public float mana;
    public float maxMana;
    public float stamina;
    public float maxStamina;
    public float attack;
    public float defense;
    public float dexterity;
    public float speed;
    public float gold;
}

/// <summary>
/// For storing dynamic stat data.
/// </summary>
//[Serializable]
//public class StatContainer
//{
//    public float health;
//    public float maxHealth;
//    public float mana;
//    public float maxMana;
//    public float stamina;
//    public float maxStamina;
//    public float attack;
//    public float defense;
//    public float dexterity;
//    public float speed;
//    public float gold;
//}
