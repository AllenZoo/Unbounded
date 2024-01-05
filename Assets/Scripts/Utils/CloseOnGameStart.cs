using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// For closing UI elements on game start.
public class CloseOnGameStart : MonoBehaviour
{
    // Reference to the UI elements you want to close
    public GameObject[] uiElements;

    void Start()
    {
        // Loop through each UI element and close it
        foreach (GameObject uiElement in uiElements)
        {
            if (uiElement != null)
            {
                uiElement.SetActive(false);
            }
        }
    }

}
