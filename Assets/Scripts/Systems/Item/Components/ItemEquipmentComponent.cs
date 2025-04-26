using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEquipmentComponent : IItemComponent
{
    [Required]
    public EquipmentType equipmentType;


    public ItemEquipmentComponent(EquipmentType equipmentType) {
        this.equipmentType = equipmentType;
    }

    public IItemComponent DeepClone()
    {
        return new ItemEquipmentComponent(equipmentType);
    }
}
