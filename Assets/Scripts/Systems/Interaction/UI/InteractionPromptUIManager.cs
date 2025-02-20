using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractionPromptUIManager : MonoBehaviour
{
    [Required]
    [SerializeField]
    private SO_InteractablePromptData interactablePromptData;

    [Required]
    [SerializeField]
    private InteractablePromptData onInitData;

    [Required]
    [SerializeField] 
    private GameObject displayObject;

    [Required]
    [SerializeField]
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
            displayObject.SetActive(false);
            return;
        }

        // If we reach here, we should display data.
        displayObject.SetActive(true);
        textDisplay.text = interactablePromptData.Data.message;
    }
}
