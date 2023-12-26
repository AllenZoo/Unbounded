using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// Script that handles the logic of opening the UI element assigned to the close button.
public class OpenButton : MonoBehaviour
{
    [SerializeField] private GameObject uiElementToOpen;

    private void Start()
    {
        Assert.IsNotNull(uiElementToOpen, "Open Button logic requires an UI element to open.");
    }

    public void OpenUIElement()
    {
        uiElementToOpen.SetActive(true);
    }
}
