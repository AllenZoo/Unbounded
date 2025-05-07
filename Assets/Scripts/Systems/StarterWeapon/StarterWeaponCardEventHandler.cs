using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Script to handle OnClick, hover events, etc. on the Starter Weapon Card.
/// Should only handle logic of passing Starter Weapon Card to any listeners for these events.
/// Hover animations should be hanlded by MenuEventSystemHandler.cs
/// </summary>
[RequireComponent(typeof(StarterWeaponCardView))]
public class StarterWeaponViewEventHandler : MonoBehaviour,
    IPointerClickHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{

    public Action<StarterWeaponCardView> OnUpgradeCardClicked;

    private StarterWeaponCardView cardView;

    private void Awake()
    {
        cardView = GetComponent<StarterWeaponCardView>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Debug.Log($"Clicked on upgrade card: {view.name}");
        // Example: Trigger selection, animation, or callback
        OnUpgradeCardClicked?.Invoke(cardView);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Debug.Log($"Hover enter: {view.name}");
        // Example: highlight background, play sound
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Debug.Log($"Hover exit: {view.name}");
        // Example: remove highlight
    }
}