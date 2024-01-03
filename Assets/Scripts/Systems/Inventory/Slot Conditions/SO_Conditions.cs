using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For use with SerializedDictionary since it cannot serialize list of interfaces as values.
[CreateAssetMenu(fileName = "New Conditions", menuName = "Inventory/Slot Conditions")]
public class SO_Conditions : ScriptableObject
{
    // Temp fix. TODO: find better way of doing this.
    public bool hasWeaponCondition = false;


    [SerializeReference]
    public List<ICondition> conditions = new List<ICondition>();

    private WeaponSlotCondition wsc = new WeaponSlotCondition();

    private void OnValidate()
    {
        if (hasWeaponCondition)
        {
            conditions.Add(wsc);
        } else
        {
            if (conditions.Contains(wsc))
            {
                conditions.Remove(wsc);
            }
        }
    }
}
