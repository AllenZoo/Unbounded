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
    }

    private void Start()
    {
        EventBinding<ItemDescReqEvent> itemDescReqBinding = new EventBinding<ItemDescReqEvent>(OnItemDescReqEvent);
        EventBus<ItemDescReqEvent>.Register(itemDescReqBinding);
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
}
