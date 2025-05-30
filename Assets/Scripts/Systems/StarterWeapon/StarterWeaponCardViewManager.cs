using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Handles the interaction between the StarterWeaponCardView and the data model (StarterWeaponData).
/// 
/// This component is responsible for receiving weapon data and passing it to the view for display.
/// It acts as a controller in a simplified MVC pattern.
/// </summary>
/// <remarks>
/// This component requires a <see cref="StarterWeaponCardView"/> to function correctly.
/// </remarks>
[RequireComponent(typeof(StarterWeaponCardView))]
public class StarterWeaponCardViewManager : CardViewManagerBase<StarterWeaponData>
{
    /// <summary>
    /// Reference to the card view that displays the UI elements.
    /// </summary>
    [Required, SerializeField] private StarterWeaponCardView cardView;

    /// <summary>
    /// Data used to populate the card view. Displayed as read-only in the inspector.
    /// </summary>
    [SerializeField, ReadOnly] private StarterWeaponData cardData;

    private void Awake()
    {
        cardView = GetComponent<StarterWeaponCardView>();
        Assert.IsNotNull(cardView);
    }

    /// <summary>
    /// Sets the weapon data and updates the card view accordingly.
    /// </summary>
    /// <param name="cardData">The weapon data to apply to the card view.</param>
    public override void SetCardData(StarterWeaponData cardData)
    {
        this.cardData = cardData;
        cardView.SetData(cardData);
    }
}
