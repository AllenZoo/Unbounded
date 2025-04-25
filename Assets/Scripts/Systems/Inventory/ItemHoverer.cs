using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

/// <summary>
/// Class that sets an image when item is selected.
/// </summary>
public class ItemHoverer : MonoBehaviour
{
    [Required, SerializeField] private ItemSelectionContext context;
    [Required, SerializeField] private GameObject displayUI; // The actual object we toggle on and off, depending on the selection context.
    [Required, SerializeField, ValidateInput(nameof(ValidateDisplayImage), "displayImage must be a child of displayUI.")]
    private Image displayImage; // The image reference we modify the sprite of. Should be found in displayUI.

    private void Awake()
    {
        Assert.IsNotNull(context, "Warning Item Selection Context is null!");

        Assert.IsNotNull(displayImage, "ItemHoverer needs an image reference to display item sprites.");
        Assert.IsFalse(displayImage.raycastTarget, "ItemHoverer image should not be a raycast target.");
        Assert.IsTrue(displayImage.preserveAspect, "ItemHoverer image should preserve aspect ratio.");
    }

    private void Start()
    {
        context.OnItemSelection += OnItemSelectionEvent;
        Rerender();
    }

    public void OnItemSelectionEvent(ItemSelectionContext context)
    {
        Rerender();
    }

    public void Rerender()
    {
        if (context == null)
        {
            Debug.LogWarning("Item selection context is null!");
            return;
        }

        displayUI.gameObject.SetActive(context.ShouldDisplay);
        displayImage.sprite = context.ItemSprite;
        transform.rotation = Quaternion.Euler(0, 0, context.RotOffset);
    }

    #region Validators
    private bool ValidateDisplayImage(Image img)
    {
        if (img == null || displayUI == null) return false; // Ensures both fields are assigned
        return img.transform.IsChildOf(displayUI.transform); // Checks if displayImage is a child of displayUI
    }
    #endregion
}
