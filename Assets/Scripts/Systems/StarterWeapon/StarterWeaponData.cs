using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new starter weapon card", menuName = "System/StarterWeapon/StarterWeaponCard")]
public class StarterWeaponData : ScriptableObject
{
    [field: SerializeField] public string WeaponName { get; private set; }
    [field: SerializeField] public IconData Icon { get; private set; }
    [field: SerializeField, TextArea(3, 8)] public string Description { get; private set; }
    [field: SerializeField] public Item Item { get; private set; }
}
