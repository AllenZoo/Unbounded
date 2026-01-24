using NUnit.Framework;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

public class ModalController : MonoBehaviour
{
    [Tooltip("Data for configuring the modal dialog.")]
    [Required, SerializeField] private ModalData modalData;
    [Required, SerializeField] private UIDocument modalUIDocument;


    private VisualElement mainContainer;
    private void Start()
    {
        Assert.IsNotNull(modalData, "ModalData is not assigned.");
        Assert.IsNotNull(modalUIDocument, "Modal UIDocument is not assigned.");

        mainContainer = modalUIDocument.rootVisualElement.Q<VisualElement>("Centerer");

        if (mainContainer == null) Debug.LogError("Main container 'Centerer' not found in the UIDocument. Check if there is name mismatch!");

        mainContainer.dataSource = modalData;

        VisualElement confirmButton = mainContainer.Q<VisualElement>("ConfirmButton");
        VisualElement cancelButton = mainContainer.Q<VisualElement>("CancelButton");

        confirmButton.RegisterCallback<ClickEvent>(evt => OnConfirmButtonClicked());
        cancelButton.RegisterCallback<ClickEvent>(evt => OnCancelButtonClicked());
    }

    private void OnConfirmButtonClicked()
    {
        Debug.Log("Confirm button clicked.");
        mainContainer.visible = false;
    }

    private void OnCancelButtonClicked()
    {
        Debug.Log("Cancel button clicked.");
        mainContainer.visible = false;
    }
}
