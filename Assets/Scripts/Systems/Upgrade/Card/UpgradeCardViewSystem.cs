using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

/// <summary>
/// Handles instantiating Upgrade Card Views, and handling its events.
/// </summary>
[RequireComponent(typeof(MenuEventSystemHandler))]
public class UpgradeCardViewSystem : PageUI
{
    [SerializeField] private MenuEventSystemHandler menuEventSystemHandler;
    //[Tooltip("The object we will toggle on and off depending on active state.")]
    //[SerializeField] private GameObject displayUI;
    [SerializeField] private Transform cardParent;
    [SerializeField] private UpgradeCardViewInitializer pfb;

    private Dictionary<GameObject, UpgradeCardData> pfbToDataMap = new();
    private EventBinding<OnDisplayUpgradeCardsRequest> onUpgradeCardRequestBinding;

    protected override void Awake()
    {
        base.Awake();
        menuEventSystemHandler = GetComponent<MenuEventSystemHandler>();

        Assert.IsNotNull(pfb);
        Assert.IsNotNull(menuEventSystemHandler);

        // Subscribe to event.
        onUpgradeCardRequestBinding = new EventBinding<OnDisplayUpgradeCardsRequest>(OnDisplayUpgradeCardsRequestEvent);
        EventBus<OnDisplayUpgradeCardsRequest>.Register(onUpgradeCardRequestBinding);

        ClearChildren();

    }

    private void OnDestroy()
    {
        EventBus<OnDisplayUpgradeCardsRequest>.Unregister(onUpgradeCardRequestBinding);
    }

    private void OnDisplayUpgradeCardsRequestEvent(OnDisplayUpgradeCardsRequest e) {
        ClearChildren();
        foreach (var cardData in e.upgradeCards)
        {
            CreateCard(cardData);
            
        }
        ToggleVisibility(true);

        // TODO: pause player input/pause game. (send request)
    }

    private void CreateCard(UpgradeCardData cardData)
    {
        UpgradeCardViewInitializer cardViewInit = Instantiate(pfb, cardParent);
        cardViewInit.SetData(cardData);
        pfbToDataMap.Add(cardViewInit.gameObject, cardData);

        var selectable = cardViewInit.GetComponent<Selectable>();
        if (selectable != null)
        {
            menuEventSystemHandler.RegisterSelectable(selectable);
        } else
        {
            Debug.LogError("Upgrade Card View is missing a Selectable component! (eg. missing Button component)");
        }

        // Subscribe to each card's OnClick event.
        var eventHandler = cardViewInit.GetComponent<UpgradeCardViewEventHandler>();
        if (eventHandler != null)
        {
            eventHandler.OnUpgradeCardClicked += OnUpgradeCardClicked;
        }
    }

    private void ClearChildren()
    {
        foreach (Transform child in cardParent)
        {
            Destroy(child.gameObject);
        }
            
        pfbToDataMap.Clear();
        menuEventSystemHandler.Selectables.Clear();
        ClosePage();
    }

    private void OnUpgradeCardClicked(UpgradeCardView cardView)
    {
        UpgradeCardData cardData = pfbToDataMap[cardView.gameObject];

        if (cardData != null)
        {
            EventBus<OnUpgradeCardApplyEffect>.Call(new OnUpgradeCardApplyEffect() { cardData = cardData });
        } else
        {
            Debug.LogError("UpgradeCardView reference object does not exist in map.");
        }
        
        // Close page after selection
        ClosePage();
    }

}
