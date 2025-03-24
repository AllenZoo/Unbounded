using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;


// Script that handles the logic of closing the UI element assigned to the close button.
public class CloseButton : MonoBehaviour
{
    [SerializeField] private GameObject uiElementToClose;

    private void Start()
    {
        Assert.IsNotNull(uiElementToClose, "Close Button logic requires an UI element to close.");
    }

    public void CloseUIElement()
    {
        uiElementToClose.SetActive(false);
    }
}
