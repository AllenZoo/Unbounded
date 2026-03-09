using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data class that the Item Descriptor bases the View on.
/// </summary>
public class ItemDescViewConfig
{
    public string Name;
    public string Description;


    public float Damage; // in Damage: 50 (2 + 23 + 25), this is the 50
    public float BaseAtk; // // in Damage: 50 (2 + 23 + 25), this is the 2
    public float BonusAtk; // in Damage: 50 (2 + 23 + 25), this is the 23
    public double PercentageDamageIncrease; // in Damage: 50 (2 + 23 + 25), this is the 100%
    public float DamageIncreaseFromPercent; // in Damage: 50 (2 + 23 + 25), this is the 25
    
    /// <summary>
    /// In terms of m/s
    /// </summary>
    public float ProjectileSpeed;

    public float ProjectileRange;

    /// <summary>
    /// In terms of projectiles/second
    /// </summary>
    public float FireRate;

    public int NumProjectilesPerAttack;

    /// <summary>
    /// True for anything that can be upgraded (weapons). False for stuff that cant be. 
    /// This field should control whether we display a ViewUpgrades Button.
    /// </summary>
    public bool CanViewUpgrades;

    public List<BonusStatEntry> BonusStats;

    /// <summary>
    /// Weapon traits. Eg piercing attacks, multi-attacks.
    /// </summary>
    public List<string> Traits;
    
    public ItemDescViewConfig()
    {

    }
}

public struct BonusStatEntry
{
    public Stat stat;
    public float value;
}
