using UnityEngine;
using UnityEngine.Assertions;


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
