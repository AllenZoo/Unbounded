using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ItemUpgradeComponent : IItemComponent
{
    // TODO: change to keep track of upgrade cards.
    [Tooltip("Keeps track of the upgrades that have been applied to the item. Does not affect stats, just keeps track for history.")]
    public List<UpgradeCardData> cards = new List<UpgradeCardData>();

    //TODO: refactor this using the new modifier system. Make sure this is private
    //public List<StatModifierEquipment> upgradeStatModifiers = new List<StatModifierEquipment>();

    [Tooltip("The list of modifiers added to the item via upgrades.")]
    private List<IUpgradeModifier> upgradeModifiers = new List<IUpgradeModifier>();

    #region Constructors
    public ItemUpgradeComponent()
    {
        upgradeModifiers = new List<IUpgradeModifier>();
    }

    public ItemUpgradeComponent(List<UpgradeCardData> cards)
    {
        this.upgradeModifiers = new List<IUpgradeModifier>();

        // Note: references will point to same card, list is different.
        //       Since we shouldn't modify the cards themselves, this shallow copy should be fine.
        this.cards = new List<UpgradeCardData>(cards);
    }

    public virtual void Init()
    {
        // Debug.Log("Initializing upgrade component!");
        // Extract modifiers from cards and add to modifiers list
        upgradeModifiers = cards
            .SelectMany(card => card.mods)
            .Select(mod => mod.modifier)
            .ToList();
    }
    #endregion

    #region Modifier Logic
    public List<IUpgradeModifier> GetUpgradeModifiers()
    {
        return upgradeModifiers;
    }

    public void AddUpgrade(IUpgradeModifier upgradeModifier)
    {
        upgradeModifiers.Add(upgradeModifier);
    }

    public void RemoveUpgrade(IUpgradeModifier upgradeModifier) {
        upgradeModifiers.Remove(upgradeModifier);
    }
    #endregion

    public IItemComponent DeepClone()
    {
        return new ItemUpgradeComponent(cards);
    }


    #region Equals + Hash
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        // Check if lists match each other regardless of order.
        ItemUpgradeComponent other = obj as ItemUpgradeComponent;
        bool isEqual = upgradeModifiers.All(other.upgradeModifiers.Contains) && upgradeModifiers.Count == other.upgradeModifiers.Count;
        return isEqual;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(upgradeModifiers.GetHashCode());
    }
    #endregion
}
