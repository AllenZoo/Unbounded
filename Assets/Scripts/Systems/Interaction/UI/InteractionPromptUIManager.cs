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
        interactablePromptData.OnDataChanged += Rerender;
        interactablePromptData.SetData(onInitData);
        Rerender();
    }

    private void Rerender()
    {
        if (!interactablePromptData.Data.shouldDisplayPrompt) {
            //displayObject.SetActive(false);
            canvas.enabled = false;
            return;
        }

        // If we reach here, we should display data.
        canvas.enabled = true;
        displayObject.SetActive(true);
        textDisplay.text = interactablePromptData.Data.message;
    }
}
