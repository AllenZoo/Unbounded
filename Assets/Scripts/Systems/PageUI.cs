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

[RequireComponent(typeof(Collider2D))]
public class PageUI : MonoBehaviour, IUIPage
{
    [Tooltip("The canvas that contains the UI elements whose visibility will be controlled by this script.")]
    [Required][SerializeField] private Canvas canvas;

    [Required][SerializeField] private Collider2D uiCollider;

    // Serialized for debugging
    [SerializeField] private bool isBlocked = false;

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
    }

    private void OnEnable()
    {
        UIOverlayManager.Instance.BringToFront(this);
    }
    private void Start()
    {
        UIOverlayManager.Instance.AddUIPage(this);
    }

    public Canvas GetCanvas()
    {
        return canvas;
    }

    /// <summary>
    /// Finds all colliders colliding with this page's collider and checks if this page is blocked by another page.
    /// Updates the isBlocked variable accordingly.
    /// </summary>
    private void HandleBlockedStatus()
    {
        // Check if this page is blocked by another page
        List<Collider2D> collisions = new List<Collider2D>();
        uiCollider.OverlapCollider(new ContactFilter2D(), collisions);

        isBlocked = false;
        foreach (Collider2D collision in collisions)
        {
            PageUI pageUI = collision.GetComponent<PageUI>();
            if (collision.tag == "UI" && collision != null)
            {
                if (!UIOverlayManager.Instance.IsPageInFrontOfOther(this, pageUI))
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
    /// If the page is blocked, move it to the top of the UI stack.
    /// Otherwise, close the page.
    /// </summary>
    private void MoveToTopOrClose()
    {
        if (isBlocked)
        {
            UIOverlayManager.Instance.BringToFront(this);
        }
        else
        {
            ToggleVisibility(false);
        }
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


