using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The intention of this class is to return the proper asset based on the commission EquipmentType.
/// </summary>
public class CommissionAssetGetter : Singleton<CommissionAssetGetter>
{
    [SerializeField] private Dictionary<EquipmentType, Sprite> equipmentSpriteMap;

    [Tooltip("The sprite to display when we have not added a EquipmentType Sprite pairing for that EquipmentType yet.")]
    [SerializeField] private Sprite placeHolderNullSprite;


    public Sprite GetEquipmentSprite(EquipmentType equipmentType)
    { 
        if (equipmentSpriteMap.ContainsKey(equipmentType))
        {
            return equipmentSpriteMap[equipmentType];
        }

        return placeHolderNullSprite;
    }
}
