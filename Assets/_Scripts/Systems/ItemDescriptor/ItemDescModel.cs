using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data class that the Item Descriptor bases the View on.
/// </summary>
public class ItemDescModel
{
    public string Name;
    public string Description;
    public float BaseAtk; // TODO-OPT: prob not necessary to display this, since player's probably care more about final ATK anyways.
    public float FinalAtk;
    public double PercentageDamageIncrease;

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
    
    public ItemDescModel()
    {

    }
}

public struct BonusStatEntry
{
    public Stat stat;
    public float value;
}
