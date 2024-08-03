using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUIPage
{
    Canvas GetCanvas();
}

public class UIOverlayManager : Singleton<UIOverlayManager>
{
    private List<IUIPage> uiPages = new List<IUIPage>();

    // Method to add a new UI page
    public void AddUIPage(IUIPage uiPage)
    {
        // Check if the page is already in the list
        if (!uiPages.Contains(uiPage))
        {
            uiPages.Add(uiPage);
        }

        // Bring the new UI page to the front
        BringToFront(uiPage);
    }

    // Method to remove a UI page
    public void RemoveUIPage(IUIPage uiPage)
    {
        if (uiPages.Contains(uiPage))
        {
            uiPages.Remove(uiPage);
        }
    }

    // Method to bring a UI page to the front
    public void BringToFront(IUIPage uiPage)
    {
        if (uiPages.Contains(uiPage))
        {
            // Remove and re-add the page to the end of the list
            uiPages.Remove(uiPage);
            uiPages.Add(uiPage);

            // Update the sorting order of all pages
            for (int i = 0; i < uiPages.Count; i++)
            {
                Canvas canvas = uiPages[i].GetCanvas();

                if (canvas == null)
                {
                    Debug.LogError("Canvas is null for UI page: " + uiPages[i]);
                    continue;
                }

                canvas.overrideSorting = true;
                canvas.sortingOrder = i;
            }
        }
    }
}
