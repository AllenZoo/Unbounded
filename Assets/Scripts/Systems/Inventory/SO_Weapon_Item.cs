using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Data containing weapon_item data of a weapon item.
// Use in composition with SO_Item.
[CreateAssetMenu(fileName = "New Weapon Item", menuName = "Inventory/Weapon Item")]
public class SO_Weapon_Item : ScriptableObject
{
    public Attack attack;
    public List<IStatModifier> statModifiers = new List<IStatModifier>();

    // TODO: think of more fields that weapon items need.
}
