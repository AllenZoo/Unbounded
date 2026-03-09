using Sirenix.OdinInspector;
using UnityEngine;

public class ItemDescSystem : MonoBehaviour
{
    [SerializeField] private bool HideUIOnStart = true;
    [SerializeField] private ItemDescView view;

    [SerializeField] private bool initializeWithItemData = false;

    [ShowIf(nameof(initializeWithItemData))]
    [Required, SerializeField] private Item model; // Generally Empty Initially

    private ItemDescController controller;

    private EventBinding<ItemDescReqEvent> itemDescReqBinding;
    private EventBinding<OnInventoryModifiedEvent> inventoryModifiedEventBinding;

    private void Awake()
    {
        if (initializeWithItemData && model != null)
        {
            controller = new ItemDescController.Builder().WithItemModel(model).Build(view);
        }
        else
        {
            controller = new ItemDescController.Builder().WithoutModel().Build(view);
        }

        if (HideUIOnStart)
        {
            controller.HideView();
        }

        itemDescReqBinding = new EventBinding<ItemDescReqEvent>(OnItemDescReqEvent);
        inventoryModifiedEventBinding = new EventBinding<OnInventoryModifiedEvent>(OnInventoryModifiedEvent);
    }

    private void OnEnable()
    {
        EventBus<ItemDescReqEvent>.Register(itemDescReqBinding);
        EventBus<OnInventoryModifiedEvent>.Register(inventoryModifiedEventBinding);
    }
    
    private void OnDisable()
    {
        EventBus<ItemDescReqEvent>.Unregister(itemDescReqBinding);
        EventBus<OnInventoryModifiedEvent>.Unregister(inventoryModifiedEventBinding);
    }

    private void OnDestroy()
    {
        controller.Cleanup();
    }

    public void OnItemDescReqEvent(ItemDescReqEvent itemDescReqEvent)
    {
        if (itemDescReqEvent.display)
        {
            Item model = itemDescReqEvent.item;
            controller.UpdateModel(model);
        }
        else
        {
            controller.HideView();
        }
    }

    public void OnInventoryModifiedEvent()
    {
        // Model is an Item , not an Inventory, so we don't really need to do anything here for now.
        // But if we want the item descriptor to dynamically update based on inventory modifications
        // (e.g. if the item descriptor should reflect changes to item stats based on inventory buffs/debuffs),
        // then we can trigger an update here after refactoring model to be flexible to inventory registration.

        // e.g. we can initialize with equipment system.

        //controller.UpdateModel(model);
    }
}
