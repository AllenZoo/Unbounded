using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="new upgrade card", menuName ="System/Upgrade/UpgradeCard")]
public class UpgradeCardData : ScriptableObject
{
    public string title;
    public Sprite icon;
    public StatModifier modifier; // testing serialiation
}
