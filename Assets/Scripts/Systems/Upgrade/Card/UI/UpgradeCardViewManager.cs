using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the interaction between the UpgradeCardView and the data model (UpgradeCardData).
/// 
/// This component is responsible for receiving upgrade data and passing it to the view for display.
/// It acts as a controller in a simplified MVC pattern.
/// </summary>
/// <remarks>
/// This component requires a <see cref="UpgradeCardView"/> to function correctly.
/// </remarks>
[RequireComponent(typeof(UpgradeCardView))]
public class UpgradeCardViewManager : CardViewManagerBase<UpgradeCardData>
{
    /// <summary>
    /// Reference to the card view that displays the UI elements.
    /// </summary>
    [Required, SerializeField] private UpgradeCardView cardView;

    /// <summary>
    /// Data used to populate the card view. Displayed as read-only in the inspector.
    /// </summary>
    [SerializeField, ReadOnly] private UpgradeCardData cardData;

    private void Start()
    {
        cardView = GetComponent<UpgradeCardView>();
    }

    /// <summary>
    /// Sets the upgrade data and updates the card view accordingly.
    /// </summary>
    /// <param name="cardData">The weapon data to apply to the card view.</param>
    public override void SetCardData(UpgradeCardData data)
    {
        cardData = data;
        cardView.Render(data);
    }
}
