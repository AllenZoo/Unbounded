using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

// TODO: make this a scriptable object dictionary. Assign reference in CommissionGenerator. Add image and rot offset field to Commission.

/// <summary>
/// The intention of this class is to return the proper asset based on the commission EquipmentType.
/// </summary>
[CreateAssetMenu(fileName = "new Commission Asset Dictionary", menuName = "System/Commission/CommissionAssetDictionary")]
public class CommissionAssetDictionary : SerializedScriptableObject
{
    [SerializeField] private Dictionary<EquipmentType, CommissionAsset> equipmentSpriteMap;

    [Tooltip("The sprite to display when we have not added a EquipmentType Sprite pairing for that EquipmentType yet.")]
    [SerializeField] private Sprite placeHolderNullSprite;

    #region Dictionary Value Struct
    [Serializable]
    public struct CommissionAsset
    {
        public Sprite sprite;
        public float rotOffset;
    }
    #endregion

    public CommissionAsset GetEquipmentSprite(EquipmentType equipmentType)
    {
        if (equipmentSpriteMap.ContainsKey(equipmentType))
        {
            return equipmentSpriteMap[equipmentType];
        }

        return new CommissionAsset { sprite = placeHolderNullSprite, rotOffset = 0 };
    }
}


