using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

// TODO: add some caching system so that we don't have to constantly clear and apply modifiers. We should cache stuff for better performance.

/// <summary>
/// Handles the application of all modifiers on a Item component.
/// Stores the final modified state of each relevant weapon component.
/// Main entrypoint for other systems wanting to get the post modification state of item.
/// </summary>
public class ItemModifierMediator : IUpgradeModifierVisitor
{
    /// <summary>
    /// Event invoked whenever a new modifier is added to item.
    /// </summary>
    public Action<Item> OnModifierChange;

    private CacheMediator<StatContainer, IUpgradeModifier> statCache;

    /// <summary>
    /// Each ItemModifierMediator should have a 1 to 1 relation with an Item.
    /// </summary>
    private Item item;
    private ItemBaseStatComponent baseStatComponent;
    private ItemUpgradeComponent upgradeComponent;
    private IAttacker baseAttacker; // The base Attacker instance to create a copy of.
    private IAttacker dynamicAttacker; // Accumlator of modifier visitor. Deep copy of base attacker.

    // Holds the stats of the item after modification. The accumulator passed in the modifier visitor.
    private StatContainer statContainer;

    // Accumulator for damage modifiers.
    private double percentageDamageIncrease = 0;

    private Action onUpgradeModifierChangeHandler;

    public ItemModifierMediator(Item item)
    {
        this.item = item;
        this.Init(item);
    }
    private void Init(Item item)
    {
        if (item == null) { return; }

        baseStatComponent = item.GetComponent<ItemBaseStatComponent>();
        if (baseStatComponent != null)
        {
            statContainer = new StatContainer(baseStatComponent.BaseStats);
        }

        upgradeComponent = item.GetComponent<ItemUpgradeComponent>();
        if (upgradeComponent != null)
        {
            onUpgradeModifierChangeHandler = () => OnModifierChange?.Invoke(item);
            upgradeComponent.OnUpgradeModifierChange += onUpgradeModifierChangeHandler;
        }

        baseAttacker = item.IsEmpty() ? null : item?.Data?.attacker;
    }

    #region Mediator Query Functions
    /**
     * Note: There was a bug previously were we called query objects, and clearing previously created objects that certain
     * systems depended on. (e.g. clearing scriptable object of an attacker that was still being used). Thus these functions should be used carefully.
     * 
     */
    public double QueryPercentageDamageIncreaseTotal()
    {
        ClearModifiers(ModifierType.Damage);
        ApplyModifiers(upgradeComponent, ModifierType.Damage);
        return percentageDamageIncrease;
    }
    public Optional<StatContainer> QueryStatsBeforeModification()
    {
        if (baseStatComponent != null)
        {
            return new Optional<StatContainer>(new StatContainer(baseStatComponent.BaseStats));
        } else {
            if (Debug.isDebugBuild) Debug.LogError("Base stat component is null!");
            return new Optional<StatContainer>(null);
        }
    }
    public Optional<StatContainer> QueryStatsAfterModification()
    {
        if (baseStatComponent == null)
        {
            if (Debug.isDebugBuild) Debug.LogError("Base stat component is null!");
            return new Optional<StatContainer>(null);
        }

        ClearModifiers(ModifierType.Stat);
        ApplyModifiers(upgradeComponent, ModifierType.Stat);

        return new Optional<StatContainer>(statContainer);
    }
    public IAttacker QueryAttackerAfterModification()
    {
        ClearModifiers(ModifierType.Trait);
        ApplyModifiers(upgradeComponent, ModifierType.Trait);
        ApplyModifiers(upgradeComponent, ModifierType.Range);
        ApplyModifiers(upgradeComponent, ModifierType.ProjectileSpeed);
        return dynamicAttacker;
    }
    #endregion

    #region Modifier Application Helpers
    private enum ModifierType
    {
        Stat, // eg. + 1 ATK
        Damage, // eg. + 10% damage
        Trait, // eg. Add Weapon Piercing.
        Range, // eg. Range Increase
        ProjectileSpeed, // eg. Projectile Speed Increase
        All, // all of the above.
    }

    /// <summary>
    /// Applies the modifiers.
    /// </summary>
    /// <param name="modifiers"></param>
    private void ApplyModifiers(List<IUpgradeModifier> modifiers)
    {
        foreach (var modifier in modifiers)
        {
            modifier.Accept(this);
        }
    }

    private void ApplyModifiers(ItemUpgradeComponent component, ModifierType modifierType)
    {
        if (component == null)
        {
            Debug.Log("Failed to apply Modifiers for item with no ItemUpgradeComponent");
            return;
        }
        ApplyModifiers(GetModifiersOfType(component, modifierType));
    }

    private List<IUpgradeModifier> GetModifiersOfType(ItemUpgradeComponent component, ModifierType modifierType)
    {
        if (component == null)
        {
            Debug.Log("No upgrade component to extract upgrade modifiers from!");
            return new List<IUpgradeModifier>();
        }

        var fullList = component.GetUpgradeModifiers();

        return fullList.Where(m => modifierType switch
        {
            ModifierType.Stat => m is StatModifier,
            ModifierType.Damage => m is DamageModifier,
            ModifierType.Trait => m is TraitModifier,
            ModifierType.Range => m is RangeModifier,
            ModifierType.ProjectileSpeed => m is ProjectileSpeedModifier,
            ModifierType.All => true,
            _ => false
        }).ToList();

    }


    /// <summary>
    /// Clears all previously applied modifier.
    /// </summary>
    private void ClearModifiers(ModifierType modType)
    {
        switch (modType)
        {
            case ModifierType.Stat:
                // Recreate the stat container from base stats so we never accumulate on an already-modified container
                if (baseStatComponent != null)
                {
                    statContainer = new StatContainer(baseStatComponent.BaseStats);
                }
                else
                {
                    statContainer?.StatMediator.ClearModifiers();
                }
                break;

            case ModifierType.Damage:
                percentageDamageIncrease = 0;
                break;

            case ModifierType.Trait:
                // Ensure baseAttacker is set (lazy init, because item.Data may not have been ready in ctor)
                if (baseAttacker == null)
                {
                    if (item != null && !item.IsEmpty())
                        baseAttacker = item?.Data?.attacker;
                }

                // If there's no base attacker, nothing to build
                if (baseAttacker == null)
                {
                    dynamicAttacker = null;
                    return;
                }

                // Create new dynamic attacker if never created before.
                if (dynamicAttacker == null)
                {
                    dynamicAttacker = baseAttacker.DeepClone();
                }

                // Modify dynamic attacker back to base attacker
                else if (dynamicAttacker != null)
                {
                    // Destroy previous instantiated SOs safely
                    if (dynamicAttacker.AttackData != null)
                        UnityEngine.Object.Destroy(dynamicAttacker.AttackData);

                    if (dynamicAttacker.AttackerData != null)
                        UnityEngine.Object.Destroy(dynamicAttacker.AttackerData);

                    // Create fresh deep copy of attack/attacker SOs
                    var attackerData = ScriptableObject.Instantiate(baseAttacker.AttackerData);
                    var attackData = ScriptableObject.Instantiate(baseAttacker.AttackData);

                    dynamicAttacker.AttackerData = attackerData;
                    dynamicAttacker.AttackData = attackData;

                    // IMPORTANT: Do not create new Attacker instance. Bug will occur since EquipmentWeaponHandler --> AttackerComponent references the
                    //            dynamicAttacker created. Never destroy the dynamicAttacker object, just modify it.
                    // dynamicAttacker = new Attacker(attackerData, attackData);
                }


                break;
        }
    }
    #endregion

    #region Visiter Functions
    public virtual void Visit(StatModifier modifier)
    {
        if (statContainer == null)
        {
            if (baseStatComponent != null)
                statContainer = new StatContainer(baseStatComponent.BaseStats);
            else
            {
                if (Debug.isDebugBuild) Debug.LogError("StatModifier visited but no baseStatComponent!");
                return;
            }
        }
        statContainer.StatMediator.AddModifier(modifier);
    }

    public virtual void Visit(DamageModifier modifier)
    {
        percentageDamageIncrease += modifier.PercentageIncrease;
    }

    public virtual void Visit(TraitModifier modifier)
    {
        // Ensure dynamic attacker exists (clear/instantiate if necessary)
        if (dynamicAttacker == null)
        {
            ClearModifiers(ModifierType.Trait); // will lazy-init baseAttacker and dynamicAttacker if possible
            if (dynamicAttacker == null)
            {
                if (Debug.isDebugBuild) Debug.LogWarning("TraitModifier applied but no attacker exists to modify.");
                return;
            }
        }

        if (modifier.AddPiercing)
        {
            if (dynamicAttacker.AttackData != null)
                dynamicAttacker.AttackData.IsPiercing = true;
        }

        if (dynamicAttacker.AttackerData != null)
            dynamicAttacker.AttackerData.numAttacks += modifier.NumAtksToAdd;
    }

    public virtual void Visit(RangeModifier modifier)
    {
        if (dynamicAttacker == null)
        {
            ClearModifiers(ModifierType.Trait);
            if (dynamicAttacker == null) return;
        }

        if (dynamicAttacker.AttackData != null)
            dynamicAttacker.AttackData.Distance += modifier.RangeToAdd;
    }

    public virtual void Visit(ProjectileSpeedModifier modifier)
    {
        if (dynamicAttacker == null)
        {
            ClearModifiers(ModifierType.Trait);
            if (dynamicAttacker == null) return;
        }

        if (dynamicAttacker.AttackData != null)
            dynamicAttacker.AttackData.InitialSpeed += modifier.ProjectileSpeedToAdd;
    }
    #endregion

    #region Getters
    public Item GetItem()
    {
        return item;
    }

    public IAttacker GetAttackerAfterModification()
    {
        return dynamicAttacker;
    }
    public Optional<StatContainer> GetStatsBeforeModification()
    {
        if (baseStatComponent != null)
        {
            return new Optional<StatContainer>(new StatContainer(baseStatComponent.BaseStats));
        }
        return null;
    }
    public Optional<StatContainer> GetStatsAfterModification()
    {
        return new Optional<StatContainer>(statContainer);
    }
    public double GetPercentageDamageIncreaseTotal()
    {
        return percentageDamageIncrease;
    }
    #endregion
}
