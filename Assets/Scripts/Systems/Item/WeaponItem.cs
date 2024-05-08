using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : Item
{

    public WeaponItem()
    {
        equipBehaviour = new StandardEquipItemBehaviour(new ItemStatComponent());
        attackBehaviour = new StandardAttackItemBehaviour(new ItemAttackComponent(null));

    }

    public override Item Clone()
    {
        throw new System.NotImplementedException();
    }
}
