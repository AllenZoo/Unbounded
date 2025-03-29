using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Handles rendering the UI of the commission board.
/// </summary>
public class CommissionBoardUI : MonoBehaviour
{
    [SerializeField, Required]
    private CommissionSlotUI commissionSlotUIPfb;

    [SerializeField, Required]
    private Transform commissionSlotParent;

    // TODO-OPT: eventually refactor this (along with pendingView) although not a big deal for now.
    [SerializeField, Required]
    private GameObject activeView;

    [SerializeField, Required]
    private GameObject pendingView;

    // The data we are displaying in the UI.
    [SerializeField, Required]private CommissionsContext commissionsContext;
 
    private CommissionBoardViewStatus viewStatus = CommissionBoardViewStatus.ACTIVE; // Toggles between ACTIVE = commissions we accepted and PENDING = commissions that we have not accepted.

    // Handle pooling in here for now, but eventually when this class becomes to big refactor this logic somewhere else.
    private List<CommissionSlotUI> commissionSlotUIPool = new List<CommissionSlotUI>();
    private List<CommissionSlotUI> activeSlots = new List<CommissionSlotUI>();

    #region Lifecycle Methods
    private void Awake()
    {
        Assert.IsNotNull(commissionSlotUIPfb);
        Assert.IsNotNull(commissionSlotParent);
        Assert.IsNotNull(activeView);
        Assert.IsNotNull(pendingView);
        Assert.IsNotNull(commissionsContext);
    }

    private void OnEnable()
    {
        commissionsContext.OnCommissionContextChange += RenderCommissions;
    }

    private void OnDisable()
    {
        commissionsContext.OnCommissionContextChange -= RenderCommissions;
    }

    private void Start()
    {
        // Add all children to the pool.
        foreach (Transform child in commissionSlotParent)
        {
            if (child.GetComponent<CommissionSlotUI>() != null)
            {
                CommissionSlotUI slot = child.GetComponent<CommissionSlotUI>();
                commissionSlotUIPool.Add(slot);
            }
            child.gameObject.SetActive(false);
        }

        RenderCommissions();
    }
    #endregion


    #region Unity Button Helpers
    public void ToggleView(CommissionBoardViewStatus status)
    {
        viewStatus = status;
        RenderCommissions();
    }

    public void SetActiveCommissionView()
    {
        viewStatus = CommissionBoardViewStatus.ACTIVE;
        RenderCommissions();
    }

    public void SetPendingCommissionView()
    {
        viewStatus = CommissionBoardViewStatus.PENDING;
        RenderCommissions();
    }
    #endregion


    #region Rendering

    /// <summary>
    /// Toggles the view between active and pending commissions.
    /// Correctly renders the commissions based on the view status.
    /// </summary>
    private void RenderCommissions()
    {
        HandleViewDisplay();

        activeSlots.RemoveAll(slot =>
        {
            slot.gameObject.SetActive(false);
            commissionSlotUIPool.Add(slot);
            return true; // Remove this slot from activeSlots
        });

        if (viewStatus == CommissionBoardViewStatus.ACTIVE)
        {
            DisplayActiveCommissions();
        }
        else
        {
            // Render Pending Commissions
            DisplayPendingCommissions();
        }
    }

    private void HandleViewDisplay()
    {
        if (viewStatus == CommissionBoardViewStatus.ACTIVE) {
            activeView.SetActive(true);
            pendingView.SetActive(false);
        } else {
            activeView.SetActive(false);
            pendingView.SetActive(true);
        }
    }

    /// <summary>
    /// Helper that instantiates or enables commission slots from pool to display active commissions.
    /// </summary>
    private void DisplayActiveCommissions()
    {
        Commission activeCommission = commissionsContext.ActiveCommission;

        if (activeCommission == null)
        {
            // If no active commission don't display anything.
            //DisplayCommissions(new List<Commission>());
            return;
        }

        DisplayCommissions(new List<Commission>() { activeCommission });
    }

    /// <summary>
    /// Helper that instantiates or enables commission slots from pool to display pending commissions.
    /// </summary>
    private void DisplayPendingCommissions()
    {
        List<Commission> commissions = commissionsContext.Commissions;
        DisplayCommissions(commissions);
    }

    /// <summary>
    /// Function that displays given commissions onto UI.
    /// </summary>
    /// <param name="commissions"></param>
    private void DisplayCommissions(List<Commission> commissions)
    {
        foreach (Commission commission in commissions)
        {
            CommissionSlotUI slot;
            if (commissionSlotUIPool.Count <= 0)
            {
                // Pool empty, instantiate new slot
                slot = Instantiate(commissionSlotUIPfb, commissionSlotParent);
            }
            else
            {
                // Reuse a slot from pool
                slot = commissionSlotUIPool[0];
                commissionSlotUIPool.Remove(slot);
                slot.gameObject.SetActive(true);
            }

            activeSlots.Add(slot);
            slot.SetCommission(commission);
        }
    }
    #endregion

    /// <summary>
    /// Handles behaviour for when a commission slot is clicked.
    /// Current expected behaviour is:
    ///     1*. (Optional) Close the commission board.
    ///     2. Open the Commission Info Display.
    /// </summary>
    /// <param name="commission"></param>
    private void OnCommissionSlotUIClicked(Commission commission)
    {
        // We could just make this event a gloabl event with the event bus. This allows us to avoid having a reference
        // to the commission quest info display.  This is a bit more decoupled and allows for more flexibility.
    }
}

public enum CommissionBoardViewStatus
{
    ACTIVE,
    PENDING
}
