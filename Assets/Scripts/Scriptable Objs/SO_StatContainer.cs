using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For storing static base stat data.
/// </summary>
[CreateAssetMenu(menuName = "ScriptableObjs/Stat Container", fileName ="new stat container")]
public class SO_StatContainer : ScriptableObject, IIdentifiableSO
{
    [SerializeField, ReadOnly] private string id;
    public string ID => id;

    public float health;
    public float maxHealth;
    public float attack;
    public float defense;
    public float dexterity;
    public float speed;
    public float gold;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(id))
        {
            id = System.Guid.NewGuid().ToString();
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }
#endif
}
