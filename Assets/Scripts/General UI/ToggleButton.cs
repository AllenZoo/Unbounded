using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ToggleButton : MonoBehaviour
{
    [SerializeField] private List<GameObject> uiElementsToToggle;

    private void Start()
    {
        Assert.IsTrue(uiElementsToToggle.Count > 0, "Open Button logic requires an UI element to open.");
    }

    // Manually subscribed to Button OnClick event.
    public void ToggleUIElements()
    {
        // Check if all UI elements are active.
        // If so, deactivate all.
        // Else, activate all.
        bool allActive = true;
        foreach (GameObject uiElements in uiElementsToToggle)
        {
            if (!uiElements.activeSelf)
            {
                allActive = false;
                break;
            }
        }

        if (allActive)
        {
            foreach (GameObject uiElements in uiElementsToToggle)
            {
                uiElements.SetActive(false);
            }
        }
        else
        {
            foreach (GameObject uiElements in uiElementsToToggle)
            {
                uiElements.SetActive(true);
            }
        }
    }
}
