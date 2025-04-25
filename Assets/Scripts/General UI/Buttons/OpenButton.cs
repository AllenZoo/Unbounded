using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// Script that handles the logic of opening the UI element assigned to the close button.
public class OpenButton : MonoBehaviour
{
    [SerializeField] private List<GameObject> uiElementsToOpen;

    private void Start()
    {
        Assert.IsTrue(uiElementsToOpen.Count > 0, "Open Button logic requires an UI element to open.");
    }

    // Manually subscribed to Button OnClick event.
    public void OpenUIElements()
    {
        foreach (GameObject uiElements in uiElementsToOpen)
        {
            uiElements.SetActive(true);
        }
    }
}
