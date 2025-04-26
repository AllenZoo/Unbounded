using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Shared context of item wanting to be described. Useful as a middle man for passing info from backend to UI, or even from UI to UI.
/// Meant to replace Singleton object.
/// </summary>
[CreateAssetMenu(fileName = "new Item Descriptor Context", menuName = "System/Inventory/ItemDescriptorContext")]
public class ItemDescriptorContext : ScriptableObject
{
    public Action<ItemDescriptorContext> OnItemDescriptorChange;

    public Item Item { get { return item; } set { item = value; OnItemDescriptorChange?.Invoke(this); } }
    public bool ShouldDisplay { get { return shouldDisplay; } set { shouldDisplay = value; OnItemDescriptorChange?.Invoke(this); } }

    /// <summary>
    /// Item to call descriptor on.
    /// </summary>
    [SerializeField, ReadOnly]
    private Item item = null;


    /// <summary>
    /// Controls whether we should display the UI or not.
    /// </summary>
    [SerializeField, ReadOnly]
    private bool shouldDisplay = false;

    public void SetItemDescriptorContext(Item item, bool shouldDisplay)
    {
        this.item = item;
        this.shouldDisplay = shouldDisplay;
        OnItemDescriptorChange?.Invoke(this);
    }
}
