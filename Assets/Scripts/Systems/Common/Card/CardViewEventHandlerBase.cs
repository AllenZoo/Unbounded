using System;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class CardViewEventHandlerBase<TCardView> : MonoBehaviour,
    IPointerClickHandler,
    IPointerEnterHandler,
    IPointerExitHandler
    where TCardView : CardViewBase
{
    public Action<TCardView> OnCardViewClicked;
    private TCardView cardView;

    private void Awake()
    {
        cardView = GetComponent<TCardView>();

        if (cardView == null)
        {
            Debug.LogError($"Card View of type {typeof(TCardView).FullName} not found for event handler!");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Debug.Log($"Clicked on upgrade card: {view.name}");

        if (cardView == null)
        {
            Debug.LogWarning("CardView is missing, cannot invoke OnCardViewClicked.");
            return;
        }
        OnCardViewClicked?.Invoke(cardView);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Debug.Log($"Hover enter: {view.name}");

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Debug.Log($"Hover exit: {view.name}");

    }
}
