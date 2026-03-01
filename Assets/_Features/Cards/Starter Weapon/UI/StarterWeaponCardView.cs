using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A view component for displaying the UI elements of a StarterWeaponCard prefab.
///
/// This class is responsible for configuring the visual elements such as the title,
/// icon (with rotation), and description based on the provided <see cref="StarterWeaponData"/>.
/// It should be attached to the corresponding UI prefab in the Unity Editor.
/// </summary>
/// <remarks>
/// Expects serialized references to UI elements (TextMeshProUGUI for text, Image for the icon).
/// The <see cref="SetData"/> method should be called with valid data to populate the card.
/// </remarks>
public class StarterWeaponCardView : CardViewBase
{
    /// <summary>
    /// Text field displaying the weapon's name.
    /// </summary>
    [SerializeField] private TextMeshProUGUI title;

    /// <summary>
    /// Transform used to rotate the weapon icon based on the data's rotation offset.
    /// </summary>
    [SerializeField] private Transform iconTransform;

    /// <summary>
    /// Image component used to render the weapon icon sprite.
    /// </summary>
    [SerializeField] private Image icon;

    /// <summary>
    /// Text field displaying the weapon's description.
    /// </summary>
    [SerializeField] private TextMeshProUGUI description;

    /// <summary>
    /// Populates the UI elements with data from the given <see cref="StarterWeaponData"/> object.
    /// </summary>
    /// <param name="data">The weapon data used to set the UI fields.</param>
    public void SetData(StarterWeaponData data)
    {
        if (data == null) { return; }

        title.text = data.WeaponName;

        iconTransform.localEulerAngles = new Vector3(0, 0, data.Icon.RotOffset);
        icon.sprite = data.Icon.Icon;

        description.text = data.Description;
    }
}
