using NUnit.Framework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

public class ModalController : MonoBehaviour
{
    [Tooltip("Data for configuring the modal dialog.")]
    [Required, SerializeField] private ModalData initModalData;
    [Required, SerializeField] private UIDocument modalUIDocument;
    [Required, SerializeField] private ModalContext modalContext;
    

    private VisualElement mainContainer;

    private void Start()
    {
        Assert.IsNotNull(initModalData, "ModalData is not assigned.");
        Assert.IsNotNull(modalUIDocument, "Modal UIDocument is not assigned.");
        Assert.IsNotNull(modalContext, "ModalContext is not assigned.");

        // Get main container
        mainContainer = modalUIDocument.rootVisualElement.Q<VisualElement>("Centerer");
        if (mainContainer == null) Debug.LogError("Main container 'Centerer' not found in the UIDocument. Check if there is name mismatch!");

        VisualElement confirmButton = mainContainer.Q<VisualElement>("ConfirmButton");
        VisualElement cancelButton = mainContainer.Q<VisualElement>("CancelButton");

        confirmButton.RegisterCallback<ClickEvent>(evt => OnConfirmButtonClicked());
        cancelButton.RegisterCallback<ClickEvent>(evt => OnCancelButtonClicked());

        // Open the modal with initial data
        modalContext.Open(initModalData);
    }

    private void OnEnable()
    {
        modalContext.OnChanged += HandleModalContextChanged;
    }

    private void OnDisable()
    {
        modalContext.OnChanged += HandleModalContextChanged;
    }

    private void OnConfirmButtonClicked()
    {
        modalContext.Payload.ModalAnswerPayload.Set(true);
        mainContainer.visible = false;
    }

    private void OnCancelButtonClicked()
    {
        modalContext.Payload.ModalAnswerPayload.Set(false);
        mainContainer.visible = false;
    }

    private void HandleModalContextChanged()
    {
        mainContainer.visible = modalContext.IsOpen;
        mainContainer.dataSource = modalContext.Payload;
    }
}
