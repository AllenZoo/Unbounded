using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Class that handles the visibility of a UI page as well as the sorting order of all UI pages.
/// </summary>

public class PageUI : MonoBehaviour, IUIPage
{
    [Tooltip("The canvas that contains the UI elements whose visibility will be controlled by this script.")]
    [Required][SerializeField] private Canvas canvas;

    [Required][SerializeField] private Collider2D uiCollider;

    private bool isBlocked = false;

    private void Awake()
    {
        if (canvas == null)
        {
            canvas = GetComponent<Canvas>();
        }

        if (canvas == null)
        {
            Debug.LogError("Canvas not found in PageUI.");
        }

        uiCollider.isTrigger = true;
        canvas.overrideSorting = true;
    }

    private void OnEnable()
    {
        UIOverlayManager.Instance.BringToFront(this);
    }
    private void Start()
    {
        Debug.Log("Added Page UI: " + gameObject.name);

        UIOverlayManager.Instance.AddUIPage(this);
        
        // Note: not necessary for since we call HandleBlockedStatus in MoveToTopOrClose.
        // UIOverlayManager.OnPageOrderModified += HandleBlockedStatus;
    }

    public Canvas GetCanvas()
    {
        return canvas;
    }

    /// <summary>
    /// If the page is blocked or invisible, display it move it to the top of the UI stack.
    /// Otherwise, close the page.
    /// </summary>
    public void MoveToTopOrClose()
    {
        HandleBlockedStatus();
        if (isBlocked || !canvas.enabled)
        {
            ToggleVisibility(true);
            UIOverlayManager.Instance.BringToFront(this);
        }
        else
        {
            ToggleVisibility(false);
            // UIOverlayManager.Instance.BringToBack(this);
        }
    }


    /// <summary>
    /// Finds all colliders colliding with this page's collider and checks if this page is blocked by another page.
    /// Updates the isBlocked variable accordingly.
    /// </summary>
    private void HandleBlockedStatus()
    {
        // Check if this page is blocked by another page
        List<Collider2D> collisions = new List<Collider2D>();

        // Set up the ContactFilter2D to only include the "UI" layer (or whatever layer UI elements are on)
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(LayerMask.GetMask("UI"));
        filter.useLayerMask = true;
        filter.useTriggers = true; // Assuming UI elements might be triggers

        uiCollider.OverlapCollider(filter, collisions);

        isBlocked = false;
        foreach (Collider2D collision in collisions)
        {
            if (collision != null && collision.CompareTag("UI"))
            {
                PageUI pageUI = collision.GetComponent<PageUI>();
                if (pageUI != null && !UIOverlayManager.Instance.IsPageInFrontOfOther(this, pageUI)
                    && pageUI.GetCanvas().enabled)
                {
                    isBlocked = true;
                    return;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleBlockedStatus();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        HandleBlockedStatus();
    }

    /// <summary>
    /// Toggles the visibility of the page.
    /// </summary>
    /// <param name="isVisible"></param>
    private void ToggleVisibility(bool isVisible)
    {
        canvas.enabled = isVisible;
    }
    private void ToggleVisibility()
    {
        canvas.enabled = !canvas.enabled;
    }
}


