using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "System/Item/Item")]
[Serializable]
public class ItemData : SerializedScriptableObject, IIdentifiableSO
{
    [SerializeField, ReadOnly] private string id;
    public string ID => id;

    public string itemName;

    [JsonIgnore]
    public Sprite itemSprite;
    public float spriteRot;
    public bool isStackable;
    public string description;

    [Tooltip("For weapons that contain some attack behaviour data.")]
    [OdinSerialize, NonSerialized] // Let Odin Serialize
    public IAttacker attacker;

    // TODO:
    [SerializeReference, InlineEditor, ValueDropdown(nameof(GetItemComponentTypes))]
    public List<IItemComponent> initialComponents = new List<IItemComponent>();

    public override string ToString()
    {
        return string.Format("[Item Name: {0}]", itemName);
    }

    private static IEnumerable<object> GetItemComponentTypes()
    {
        // For Odin serialization of interfaces.
        yield return new ItemAttackContainerComponent(null);
        yield return new ItemBaseStatComponent();
        yield return new ItemUpgradeComponent();
        // yield return new ItemUpgraderComponent();
        yield return new ItemEquipmentComponent(EquipmentType.SWORD);
        //yield return new ItemValueComponent();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(id))
        {
            id = System.Guid.NewGuid().ToString();
            UnityEditor.EditorUtility.SetDirty(this);
        }

        if (attacker == null) { 
            Debug.LogWarning($"ItemData {itemName} ({id}) has no attacker assigned.");
        }
    }
#endif
}
