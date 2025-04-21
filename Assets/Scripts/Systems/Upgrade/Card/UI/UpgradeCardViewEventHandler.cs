using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


/// <summary>
/// Script to handle OnClick, hover events, etc. on the Upgrade Card.
/// </summary>
[RequireComponent(typeof(UpgradeCardView))]
public class UpgradeCardViewEventHandler : MonoBehaviour,
    IPointerClickHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [SerializeField] private UpgradeCardView view;

    private void Awake()
    {
        view = GetComponent<UpgradeCardView>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Debug.Log($"Clicked on upgrade card: {view.name}");
        // Example: Trigger selection, animation, or callback
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

