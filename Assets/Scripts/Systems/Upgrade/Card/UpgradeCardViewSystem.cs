using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

/// <summary>
/// Handles instantiating Upgrade Card Views, and handling its events.
/// </summary>
[RequireComponent(typeof(MenuEventSystemHandler))]
public class UpgradeCardViewSystem : MonoBehaviour
{
    [SerializeField] private MenuEventSystemHandler menuEventSystemHandler;
    [Tooltip("The object we will toggle on and off depending on active state.")]
    [SerializeField] private GameObject displayUI;
    [SerializeField] private Transform cardParent;
    [SerializeField] private UpgradeCardViewInitializer pfb;

    // TODO: use this
    [SerializeField, ReadOnly] private bool active = false;

    private void Awake()
    {
        menuEventSystemHandler = GetComponent<MenuEventSystemHandler>();

        Assert.IsNotNull(pfb);
        Assert.IsNotNull(menuEventSystemHandler);

        // Subscribe to event.
        EventBinding<OnDisplayUpgradeCardsRequest> onUpgradeCardRequestBinding = new EventBinding<OnDisplayUpgradeCardsRequest>(OnDisplayUpgradeCardsRequestEvent);
        EventBus<OnDisplayUpgradeCardsRequest>.Register(onUpgradeCardRequestBinding);

        ClearChildren();

    }

    private void OnDisplayUpgradeCardsRequestEvent(OnDisplayUpgradeCardsRequest e) {
        ClearChildren();

        foreach (var cardData in e.upgradeCards)
        {
            CreateCard(cardData);
        }
    }

    private void CreateCard(UpgradeCardData cardData)
    {
        UpgradeCardViewInitializer cardViewInit = Instantiate(pfb, cardParent);
        cardViewInit.SetData(cardData);

        var selectable = cardViewInit.GetComponent<Selectable>();
        if (selectable != null)
        {
            menuEventSystemHandler.RegisterSelectable(selectable);
        } else
        {
            Debug.LogError("Upgrade Card View is missing a Selectable component! (eg. missing Button component)");
        }
    }

    private void ClearChildren()
    {
        foreach (Transform child in cardParent)
        {
            Destroy(child.gameObject);
        }
            
        menuEventSystemHandler.Selectables.Clear();
    }

}
