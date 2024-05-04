using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// Stores fields relevant to Attackers.
[CreateAssetMenu(fileName = "SO_Attacker", menuName = "ScriptableObjs/AttackerData", order = 1)]
public class SO_Attacker : ScriptableObject
{
    // Didn't make sense to have this field here, since it's possible that the same attacker 
    // can attack different types of entities.
    // [Tooltip("The entity types that this attacker can damage.")]
    // public List<EntityType> TargetTypes = new List<EntityType>();

    public AttackerData data;

    private void OnValidate()
    {
        if (data.attackObj != null)
        {
            Assert.IsNotNull(data.attackObj.GetComponent<Rigidbody2D>(), "attack obj needs rb2d to set velocity in " + data.attackObj.name);
            Assert.IsNotNull(data.attackObj.GetComponent<Attack>(), "Attack Obj needs Attack component in " + data.attackObj.name);
        }
    }
}
