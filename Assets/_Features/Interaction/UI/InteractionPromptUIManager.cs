using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractionPromptUIManager : MonoBehaviour
{
    /// <summary>
    /// Canvas that holds everything displayed by IPUIManager.
    /// Used for hiding/showing the page.
    /// </summary>
    [Required, SerializeField]
    private Canvas canvas;

    [Required, SerializeField]
    private SO_InteractablePromptData interactablePromptData;

    [Required, SerializeField]
    private InteractablePromptData onInitData;

    [Required, SerializeField]
    private GameObject displayObject;

    [Required, SerializeField]
    private TextMeshProUGUI textDisplay;

    private void Start()
    {
        if (interactablePromptData != null)
        {
            interactablePromptData.OnDataChanged += Rerender;
            interactablePromptData.SetData(onInitData);
        }
        Rerender();
    }

    private void OnDestroy()
    {
        if (interactablePromptData != null)
        {
            interactablePromptData.OnDataChanged -= Rerender;
        }
    }

    private void Rerender()
    {
        if (this == null || canvas == null || interactablePromptData == null) return;

        if (!interactablePromptData.Data.shouldDisplayPrompt) {
            canvas.enabled = false;
            if (displayObject != null) displayObject.SetActive(false);
            return;
        }

        // If we reach here, we should display data.
        canvas.enabled = true;
        if (displayObject != null) displayObject.SetActive(true);
        if (textDisplay != null) textDisplay.text = interactablePromptData.Data.message;
    }
}
