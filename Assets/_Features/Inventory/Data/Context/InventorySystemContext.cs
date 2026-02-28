using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that acts as a link to an InventorySystem via Scriptable Object Global System linking.
/// </summary>
[CreateAssetMenu(fileName = "new inventory system context", menuName = "System/Inventory/InventorySystemContext")]
public class InventorySystemContext : ScriptableObject
{
    public Action OnInventorySystemChange;

    /// <summary>
    /// Should be referenced in Start.
    /// </summary>
    [field: SerializeField, ReadOnly] private InventorySystem InventorySystem;
    private bool initialized = false;

    /// <summary>
    /// Should be called in Awake.
    /// </summary>
    public void Init(InventorySystem inventorySystem)
    {
        this.InventorySystem = inventorySystem;
        OnInventorySystemChange?.Invoke();
        initialized = true;
    }

    /// <summary>
    /// Should be invoked in/after Start. Init (should) happen in Awake.
    /// </summary>
    public InventorySystem Get()
    {
        if (!initialized)
        {
            Debug.LogError("The Inventory System Context requested is not initialized yet!");
            return null;
        }
        return this.InventorySystem;
    }
}
