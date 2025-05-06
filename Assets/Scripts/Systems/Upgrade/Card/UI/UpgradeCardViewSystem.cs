using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

/// <summary>
/// Handles the creation, display, and interaction logic for upgrade card UI elements.
/// Subscribes to upgrade card events, manages instantiation of card prefabs,
/// and relays user selections to other systems.
/// </summary>
[RequireComponent(typeof(MenuEventSystemHandler))]
public class UpgradeCardViewSystem : PageUI
{
    [Tooltip("Handles registering and managing Unity UI Selectables for input navigation.")]
    [SerializeField] private MenuEventSystemHandler menuEventSystemHandler;

    [Tooltip("Parent transform where all upgrade cards will be instantiated.")]
    [SerializeField] private Transform cardParent;

    [Tooltip("Prefab used to instantiate upgrade card views.")]
    [SerializeField] private UpgradeCardViewInitializer pfb;

    // Maps instantiated card GameObjects to their associated data.
    private Dictionary<GameObject, UpgradeCardData> pfbToDataMap = new();

    // Event binding for upgrade card display requests.
    private EventBinding<OnDisplayUpgradeCardsRequest> onUpgradeCardRequestBinding;

    /// <summary>
    /// Initializes references and subscribes to upgrade card display events.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        menuEventSystemHandler = GetComponent<MenuEventSystemHandler>();

        Assert.IsNotNull(pfb, "Upgrade card prefab is not assigned.");
        Assert.IsNotNull(menuEventSystemHandler, "MenuEventSystemHandler is not assigned or attached.");

        onUpgradeCardRequestBinding = new EventBinding<OnDisplayUpgradeCardsRequest>(OnDisplayUpgradeCardsRequestEvent);
        EventBus<OnDisplayUpgradeCardsRequest>.Register(onUpgradeCardRequestBinding);

        ClearChildren();
    }

    /// <summary>
    /// Ensures base startup behavior executes.
    /// </summary>
    protected override void Start()
    {
        base.Start();
    }

    /// <summary>
    /// Unsubscribes from events to avoid memory leaks.
    /// </summary>
    private void OnDestroy()
    {
        EventBus<OnDisplayUpgradeCardsRequest>.Unregister(onUpgradeCardRequestBinding);
    }

    /// <summary>
    /// Handles incoming event to display a list of upgrade cards.
    /// Clears any existing cards and populates new ones based on the provided data.
    /// </summary>
    private void OnDisplayUpgradeCardsRequestEvent(OnDisplayUpgradeCardsRequest e)
    {
        ClearChildren();

        foreach (var cardData in e.upgradeCards)
        {
            CreateCard(cardData);
        }

        // Game state control (pause input/gameplay) could be triggered here if needed.
    }

    /// <summary>
    /// Instantiates and initializes a single upgrade card view.
    /// Registers it for navigation and click events.
    /// </summary>
    /// <param name="cardData">The data used to initialize the card view.</param>
    private void CreateCard(UpgradeCardData cardData)
    {
        var cardViewInit = Instantiate(pfb, cardParent);
        cardViewInit.SetData(cardData);
        pfbToDataMap[cardViewInit.gameObject] = cardData;

        var selectable = cardViewInit.GetComponent<Selectable>();
        if (selectable != null)
        {
            menuEventSystemHandler.RegisterSelectable(selectable);
        }
        else
        {
            Debug.LogError("Upgrade Card View is missing a Selectable component (e.g., a Button).");
        }

        var eventHandler = cardViewInit.GetComponent<UpgradeCardViewEventHandler>();
        if (eventHandler != null)
        {
            eventHandler.OnUpgradeCardClicked += OnUpgradeCardClicked;
        }
    }

    /// <summary>
    /// Destroys all currently instantiated upgrade card views and clears related data.
    /// </summary>
    private void ClearChildren()
    {
        foreach (Transform child in cardParent)
        {
            Destroy(child.gameObject);
        }

        pfbToDataMap.Clear();
        menuEventSystemHandler.Selectables.Clear();
    }

    /// <summary>
    /// Called when the user clicks on an upgrade card.
    /// Triggers the effect application and closes the upgrade UI.
    /// </summary>
    /// <param name="cardView">The clicked card view.</param>
    private void OnUpgradeCardClicked(UpgradeCardView cardView)
    {
        if (pfbToDataMap.TryGetValue(cardView.gameObject, out var cardData) && cardData != null)
        {
            EventBus<OnUpgradeCardApplyEffect>.Call(new OnUpgradeCardApplyEffect { cardData = cardData });
        }
        else
        {
            Debug.LogError("UpgradeCardView reference not found in map.");
        }

        ClosePage(); // Hide the upgrade card UI after selection.
    }
}
