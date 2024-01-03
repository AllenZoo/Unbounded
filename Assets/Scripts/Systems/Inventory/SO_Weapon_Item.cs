using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Data containing weapon_item data of a weapon item.
// Use in composition with SO_Item.
[CreateAssetMenu(fileName = "New Weapon Item", menuName = "Inventory/Weapon Item")]
public class SO_Weapon_Item : ScriptableObject
{
    public GameObject attackObj;
    public List<IStatModifier> statModifiers = new List<IStatModifier>();

    // TODO: think of more fields that weapon items need.
    private void OnValidate()
    {
        bool goodObj = true;
        // Check if attackObj contains necessary components
        if (attackObj != null)
        {
            if (attackObj.GetComponent<Attack>() == null)
            {
                Debug.LogError("Attack object needs Attack component.");
                goodObj = false;
            }
            if (attackObj.GetComponent<Collider2D>() == null)
            {
                Debug.LogError("Attack object needs Collider component.");
                goodObj = false;
            }
            if (attackObj.GetComponent<Rigidbody2D>() == null)
            {
                Debug.LogError("Attack object needs Rigidbody component.");
                 goodObj = false;
            }
        }

        if (!goodObj)
        {
            attackObj = null;
        }
    }
}
