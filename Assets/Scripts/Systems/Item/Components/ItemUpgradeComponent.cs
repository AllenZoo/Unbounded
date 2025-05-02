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

    public ItemUpgradeComponent(List<IUpgradeModifier> toBeCloned, List<UpgradeCardData> cards)
    {
        this.upgradeModifiers = new List<IUpgradeModifier>();
        //foreach (var statModifier in toBeCloned)
        //{
        //    // this.upgradeModifiers.Add(statModifier.DeepCopy());
        //}
        // TODO: double chekc this cloning logic.
        this.cards = new List<UpgradeCardData>(cards);
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
        return new ItemUpgradeComponent(upgradeModifiers, cards);
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
        throw new NotImplementedException();
        //bool isEqual = upgradeStatModifiers.All(other.upgradeStatModifiers.Contains) && upgradeStatModifiers.Count == other.upgradeStatModifiers.Count;
        return false; // Stub.
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
        //return HashCode.Combine(upgradeStatModifiers.GetHashCode());
    }
    #endregion
}
