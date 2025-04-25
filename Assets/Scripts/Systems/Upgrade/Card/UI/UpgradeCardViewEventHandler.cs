using System;
using UnityEngine;
using UnityEngine.EventSystems;


/// <summary>
/// Script to handle OnClick, hover events, etc. on the Upgrade Card.
/// Should only handle logic of passing Upgrade Card to any listeners for these events.
/// Hover animations should be hanlded by MenuEventSystemHandler.cs
/// </summary>
[RequireComponent(typeof(UpgradeCardView))]
public class UpgradeCardViewEventHandler : MonoBehaviour,
    IPointerClickHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{

    public Action<UpgradeCardView> OnUpgradeCardClicked;

    private UpgradeCardView cardView;

    private void Awake()
    {
        cardView = GetComponent<UpgradeCardView>();
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

