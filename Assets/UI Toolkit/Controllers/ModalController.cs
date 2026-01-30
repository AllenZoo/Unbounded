using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

public class ModalController : MonoBehaviour
{
    [Required, SerializeField] private bool closeOnStart = true;
    [Required, SerializeField] private UIDocument modalUIDocument;
    [Required, SerializeField] private ModalContext modalContext;

    private VisualElement mainContainer;

    private void Start()
    {
        mainContainer = modalUIDocument.rootVisualElement.Q<VisualElement>("Centerer");

        VisualElement confirmButton = mainContainer.Q<VisualElement>("ConfirmButton");
        VisualElement cancelButton = mainContainer.Q<VisualElement>("CancelButton");

        confirmButton.RegisterCallback<ClickEvent>(_ => modalContext.Resolve(true));
        cancelButton.RegisterCallback<ClickEvent>(_ => modalContext.Resolve(false));

        if (closeOnStart) modalContext.Close();
    }

    private void OnEnable()
    {
        modalContext.OnChanged += HandleModalContextChanged;
    }

    private void OnDisable()
    {
        modalContext.OnChanged -= HandleModalContextChanged;
    }

    private void HandleModalContextChanged()
    {
        mainContainer.visible = modalContext.IsOpen;
        mainContainer.dataSource = modalContext.Payload;
    }
}
