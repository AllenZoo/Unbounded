using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

// Class that holds data for inventory mouse hoverers.
public class ItemHoverer : MonoBehaviour
{
    [SerializeField] private Image displayImage;

    private void Awake()
    {
        Assert.IsNotNull(displayImage, "ItemHoverer needs image reference to display item.");

        // Check that image is not a raycast target and also has perserve aspect ratio.
        Assert.IsFalse(displayImage.raycastTarget, "ItemHoverer image should not be a raycast target.");
        Assert.IsTrue(displayImage.preserveAspect, "ItemHoverer image should preserve aspect ratio.");
    }

    // RotOffset for rotating the transform of the object
    public void SetItemSprite(Sprite sprite, float rotOffset)
    {
        displayImage.sprite = sprite;
        transform.rotation = Quaternion.Euler(0, 0, rotOffset);
    }
}
