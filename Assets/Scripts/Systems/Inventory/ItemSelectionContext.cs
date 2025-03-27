using Sirenix.OdinInspector;
using System;
using UnityEngine;

/// <summary>
/// Shared context of item being selected. Useful for displaying item that follows mouse, imitating an item being dragged.
/// </summary>
[CreateAssetMenu(fileName ="new Item Selection Context", menuName ="System/Inventory/ItemSelectionContext")]
public class ItemSelectionContext : ScriptableObject
{
    public Action<ItemSelectionContext> OnItemSelection;

    public Sprite ItemSprite { get { return itemSprite; } set { itemSprite = value; OnItemSelection?.Invoke(this); } }
    public float RotOffset { get { return rotOffset; } set { rotOffset = value; OnItemSelection?.Invoke(this); } }
    public bool ShouldDisplay { get { return shouldDisplay; } set { shouldDisplay = value; OnItemSelection?.Invoke(this); } }

    /// <summary>
    /// Item sprite we will display on Item Hoverer.
    /// </summary>
    [SerializeField, ReadOnly] 
    private Sprite itemSprite = null;

    /// <summary>
    /// Rotation we apply to the transform object of the image rendering the sprite.
    /// </summary>
    [SerializeField, ReadOnly]
    private float rotOffset = 0;

    /// <summary>
    /// Controls whether we should display the UI or not.
    /// </summary>
    [SerializeField, ReadOnly]
    private bool shouldDisplay = false;

    public void SetItemSelectionContext(Sprite sprite, float rotOffset, bool shouldDisplay)
    {
        this.itemSprite = sprite;
        this.rotOffset = rotOffset;
        this.shouldDisplay = shouldDisplay;
        OnItemSelection?.Invoke(this);
    }
}
