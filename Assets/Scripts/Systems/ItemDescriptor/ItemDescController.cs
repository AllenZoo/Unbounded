using UnityEngine;

public class ItemDescController : MonoBehaviour
{
    [SerializeField] private ItemDescView view;

    private ItemDescModel model;

    private void Awake()
    {
        if (view == null)
        {
            view = GetComponentInChildren<ItemDescView>();
        }
    }

    private void Start()
    {
        EventBinding<ItemDescReqEvent> itemDescReqBinding = new EventBinding<ItemDescReqEvent>(OnItemDescReqEvent);
        EventBus<ItemDescReqEvent>.Register(itemDescReqBinding);
    }

    public void OnItemDescReqEvent(ItemDescReqEvent itemDescReqEvent)
    {
        if (itemDescReqEvent.display)
        {
            ItemDescModel model = ItemDataConverter.ConvertFromItem(itemDescReqEvent.item);
            Show(model);
        } else
        {
            Hide();
        }
    }

    /// <summary>
    /// Injects the model data and shows the descriptor.
    /// </summary>
    public void Show(ItemDescModel newModel)
    {
        model = newModel;

        if (model == null)
        {
            Debug.LogWarning("Tried to show ItemDesc with null model.");
            return;
        }

        view.DisplayView(model);
    }

    /// <summary>
    /// Hides the descriptor view.
    /// </summary>
    public void Hide()
    {
        view.HideView();
        model = null;
    }

    /// <summary>
    /// Example hook for when an item is hovered.
    /// </summary>
    public void OnItemHovered(ItemDescModel hoveredModel)
    {
        Show(hoveredModel);
    }

    /// <summary>
    /// Example hook for when hover ends.
    /// </summary>
    public void OnItemUnhovered()
    {
        Hide();
    }
}
