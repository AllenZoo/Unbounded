using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUIPage
{
    Canvas GetCanvas();
}

public class UIOverlayManager : Singleton<UIOverlayManager>
{
    public static event Action OnPageOrderModified;

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
        OnPageOrderModified?.Invoke();
    }

    // Method to remove a UI page
    public void RemoveUIPage(IUIPage uiPage)
    {
        if (uiPages.Contains(uiPage))
        {
            uiPages.Remove(uiPage);
        }
        OnPageOrderModified?.Invoke();
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

                // canvas.overrideSorting = true;
                canvas.sortingOrder = i;
            }
        }
        OnPageOrderModified?.Invoke();
    }

    // Method to bring a UI page to the back
    public void BringToBack(IUIPage uIPage)
    {
        if (uiPages.Contains(uIPage))
        {
            // Remove and re-add the page to the end of the list
            uiPages.Remove(uIPage);
            uiPages.Insert(0, uIPage);
            // Update the sorting order of all pages
            for (int i = 0; i < uiPages.Count; i++)
            {
                Canvas canvas = uiPages[i].GetCanvas();
                if (canvas == null)
                {
                    Debug.LogError("Canvas is null for UI page: " + uiPages[i]);
                    continue;
                }
                // canvas.overrideSorting = true;
                canvas.sortingOrder = i;
            }
        }
        OnPageOrderModified?.Invoke();
    }

    // Method to check if a UI page is in front
    public bool IsPageInFrontOfAll(IUIPage uiPage)
    {
        if (uiPages.Count > 0)
        {
            return uiPages[uiPages.Count - 1] == uiPage;
        }
        return false;
    }

    /// <summary>
    /// Check if a UI page is in front of another UI page
    /// </summary>
    /// <param name="uiPage"></param>
    /// <param name="uiPage2"></param>
    /// <returns>true if uiPage is in front of uiPage2</returns>
    public bool IsPageInFrontOfOther(IUIPage uiPage, IUIPage uiPage2)
    {
        return uiPages.IndexOf(uiPage) > uiPages.IndexOf(uiPage2);
    }
}
