using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public abstract class CardViewSystemBase<TCardData, TDisplayEvent, TApplyEvent> : PageUI
    where TDisplayEvent : IEvent
    where TApplyEvent : IEvent, new()
{
    [SerializeField] protected MenuEventSystemHandler menuEventSystemHandler;
    [SerializeField] protected Transform cardParent;
    [SerializeField] protected CardViewManagerBase<TCardData> pfb;

    protected Dictionary<GameObject, TCardData> pfbToDataMap = new();

    private EventBinding<TDisplayEvent> onCardRequestBinding;

    protected override void Awake()
    {
        base.Awake();
        menuEventSystemHandler = GetComponent<MenuEventSystemHandler>();

        Assert.IsNotNull(pfb, "Card prefab is not assigned.");
        Assert.IsNotNull(menuEventSystemHandler, "MenuEventSystemHandler is not assigned or attached.");

        onCardRequestBinding = new EventBinding<TDisplayEvent>(OnDisplayRequestEvent);
        EventBus<TDisplayEvent>.Register(onCardRequestBinding);

        ClearChildren();
    }

    protected virtual void OnDestroy()
    {
        EventBus<TDisplayEvent>.Unregister(onCardRequestBinding);
    }

    private void OnDisplayRequestEvent(TDisplayEvent e)
    {
        ClearChildren();
        foreach (var cardData in GetCardListFromEvent(e))
        {
            CreateCard(cardData);
        }
    }
    protected virtual void CreateCard(TCardData cardData)
    {
        var cardViewInit = Instantiate(pfb, cardParent);
        cardViewInit.SetCardData(cardData);
        pfbToDataMap[cardViewInit.gameObject] = cardData;

        var selectable = cardViewInit.GetComponent<Button>();
        if (selectable != null)
        {
            menuEventSystemHandler.RegisterSelectable(selectable);
        }

        var eventHandler = cardViewInit.GetComponent<UpgradeCardViewEventHandler>();
        if (eventHandler != null)
        {
            eventHandler.OnUpgradeCardClicked += OnCardClicked;
        }
    }
    protected virtual void ClearChildren()
    {
        foreach (Transform child in cardParent)
        {
            Destroy(child.gameObject);
        }

        pfbToDataMap.Clear();
        menuEventSystemHandler.Selectables.Clear();
    }
    private void OnCardClicked(UpgradeCardView cardView)
    {
        if (pfbToDataMap.TryGetValue(cardView.gameObject, out var cardData))
        {
            EventBus<TApplyEvent>.Call(CreateApplyEvent(cardData));
        }

        ClosePage();
    }

    protected abstract IEnumerable<TCardData> GetCardListFromEvent(TDisplayEvent e);
    protected abstract TApplyEvent CreateApplyEvent(TCardData cardData);
}

