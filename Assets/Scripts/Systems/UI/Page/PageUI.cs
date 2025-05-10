using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the visibility and sorting order of a single UI page within the UI overlay system.
/// This component ensures that the page can be shown, hidden, or brought to the front,
/// and checks whether it is visually blocked by another UI page.
/// </summary>
[RequireComponent(typeof(Canvas), typeof(Collider2D))]
public class PageUI : MonoBehaviour, IUIPage
{
    [Tooltip("The canvas that contains the UI elements controlled by this page.")]
    [Required, SerializeField] private Canvas canvas;

    [Tooltip("The 2D collider used to detect overlap with other UI pages.")]
    [Required, SerializeField] private Collider2D uiCollider;

    [Tooltip("Optional reference to a PageUIContext for indirect initialization or communication.")]
    [SerializeField] private PageUIContext pageUIContext;

    private bool isBlocked = false;

    #region Unity Lifecycle

    /// <summary>
    /// Initializes required components and ensures canvas settings are configured.
    /// </summary>
    protected virtual void Awake()
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

    /// <summary>
    /// Brings this UI page to the front when enabled.
    /// </summary>
    protected virtual void OnEnable()
    {
        UIOverlayManager.Instance?.BringToFront(this);
    }

    /// <summary>
    /// Adds this UI page to the overlay manager and initializes the context, if available.
    /// </summary>
    protected virtual void Start()
    {
        Debug.Log($"Added Page UI: {gameObject.name}");

        UIOverlayManager.Instance?.AddUIPage(this);
        pageUIContext?.Init(this);
    }

    #endregion

    /// <summary>
    /// Returns the canvas associated with this page.
    /// </summary>
    public Canvas GetCanvas()
    {
        return canvas;
    }

    /// <summary>
    /// If this page is blocked or hidden, makes it visible and brings it to the top.
    /// Otherwise, closes (hides) the page.
    /// </summary>
    public void MoveToTopOrClose()
    {
        HandleBlockedStatus();

        if (isBlocked || !canvas.enabled)
        {
            ToggleVisibility(true);
            UIOverlayManager.Instance?.BringToFront(this);
        }
        else
        {
            ClosePage();
        }
    }

    /// <summary>
    /// Makes this page visible and brings it to the top of the UI stack.
    /// </summary>
    public void MoveToTop()
    {
        ToggleVisibility(true);
        UIOverlayManager.Instance?.BringToFront(this);
    }

    /// <summary>
    /// Hides this UI page from view.
    /// </summary>
    public void ClosePage()
    {
        ToggleVisibility(false);
    }

    /// <summary>
    /// Toggles the visibility of this page based on the provided value.
    /// </summary>
    /// <param name="isVisible">Whether the page should be visible.</param>
    public void ToggleVisibility(bool isVisible)
    {
        canvas.enabled = isVisible;
    }

    /// <summary>
    /// Inverts the current visibility state of this page.
    /// </summary>
    public void ToggleVisibility()
    {
        canvas.enabled = !canvas.enabled;
    }

    /// <summary>
    /// Determines whether this page is visually blocked by other UI pages
    /// by checking overlapping colliders and comparing sorting orders.
    /// </summary>
    private void HandleBlockedStatus()
    {
        List<Collider2D> collisions = new List<Collider2D>();

        var filter = new ContactFilter2D
        {
            useLayerMask = true,
            useTriggers = true
        };
        filter.SetLayerMask(LayerMask.GetMask("UI"));

        uiCollider.OverlapCollider(filter, collisions);

        isBlocked = false;

        foreach (Collider2D collision in collisions)
        {
            if (collision != null && collision.CompareTag("UI"))
            {
                PageUI otherPage = collision.GetComponent<PageUI>();
                if (otherPage != null &&
                    !UIOverlayManager.Instance.IsPageInFrontOfOther(this, otherPage) &&
                    otherPage.GetCanvas().enabled)
                {
                    isBlocked = true;
                    return;
                }
            }
        }
    }
}
