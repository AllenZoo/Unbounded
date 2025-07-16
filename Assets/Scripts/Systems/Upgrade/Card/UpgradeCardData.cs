using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="new upgrade card", menuName ="System/Upgrade/UpgradeCard")]
public class UpgradeCardData : ScriptableObject
{
    public string title;
    public Sprite icon;
    public Color cardColor; 

    public CardRarity rarity;

    public enum CardRarity
    {
        Common,
        Uncommon,
        Rare,
        Legendary
    }

    [TableList]
    public List<UpgradeModifierEntry> mods = new List<UpgradeModifierEntry>();

    [TextArea(5, 8)]
    public string description;

}

/// <summary>
/// Single object class for serializing IUpgradeModifiers. Generally don't want to Serializereference a list, so that's why we create a separate class here for UpgradeCardData.
/// </summary>
[Serializable]
public class UpgradeModifierEntry
{
    [SerializeReference, ValueDropdown(nameof(GetOperationTypes)), InlineEditor]
    public IUpgradeModifier modifier;

    public string modifierDescription = "";

    private static IEnumerable<object> GetOperationTypes()
    {
        yield return new StatModifier(Stat.ATK, new AddOperation(1f), -1);
        yield return new DamageModifier();
        yield return new TraitModifier();
        yield return new RangeModifier();
        yield return new ProjectileSpeedModifier();
    }
}