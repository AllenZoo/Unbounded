using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles rendering the UI of the commission board.
/// </summary>
public class CommissionBoardUI : MonoBehaviour
{
    [SerializeField]
    [Required] 
    private CommissionSlotUI commissionSlotUIPfb;

    [SerializeField]
    [Required]
    private Transform commissionSlotParent;

    // TODO-OPT: eventually refactor this (along with pendingView) although not a big deal for now.
    [SerializeField]
    [Required]
    private GameObject activeView;

    [SerializeField]
    [Required]
    private GameObject pendingView;

    // The data we are displaying in the UI.
    private List<Commission> activeCommissions = new List<Commission>();
    private List<Commission> pendingCommissions = new List<Commission>();
    private CommissionBoardViewStatus viewStatus = CommissionBoardViewStatus.ACTIVE;

    // Handle pooling in here for now, but eventually when this class becomes to big refactor this logic somewhere else.
    private List<CommissionSlotUI> commissionSlotUIPool = new List<CommissionSlotUI>();
    private List<CommissionSlotUI> activeSlots = new List<CommissionSlotUI>();

    private EventBinding<OnCommissionListModifiedEvent> commissionsModifiedBinding;

    private void Awake()
    {
        commissionsModifiedBinding = new EventBinding<OnCommissionListModifiedEvent>(OnCommissionListModified);
    }

    private void OnEnable()
    {
        
        EventBus<OnCommissionListModifiedEvent>.Register(commissionsModifiedBinding);
    }

    private void OnDisable()
    {
        EventBinding<OnCommissionListModifiedEvent> commissionsModifiedBinding = new EventBinding<OnCommissionListModifiedEvent>(OnCommissionListModified);
        EventBus<OnCommissionListModifiedEvent>.Unregister(commissionsModifiedBinding);
    }

    private void Start()
    {
        // For testing purposes, add some commissions.
        Commission commission1 = new Commission("Commission1", "description", 1, 2, 2, EquipmentType.BOW, new Dictionary<Stat, int>(), CommissionStatus.ACTIVE);
        Commission commission2 = new Commission("Commission2", "description", 1, 2, 2, EquipmentType.BOW, new Dictionary<Stat, int>(), CommissionStatus.ACTIVE);
        Commission commission3 = new Commission("Commission3", "description", 1, 2, 2, EquipmentType.BOW, new Dictionary<Stat, int>(), CommissionStatus.PENDING);
        Commission commission4 = new Commission("Commission4", "description", 1, 2, 2, EquipmentType.BOW, new Dictionary<Stat, int>(), CommissionStatus.PENDING);

        activeCommissions.Add(commission1);
        activeCommissions.Add(commission2);
        pendingCommissions.Add(commission3);
        pendingCommissions.Add(commission4);

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

    #region For Unity Buttons
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

    private void OnCommissionListModified(OnCommissionListModifiedEvent e)
    {
        activeCommissions = e.activeCommissions;
        pendingCommissions = e.pendingCommissions;
        RenderCommissions();
    }

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
        } else
        {
            // Render Pending Commissions
            DisplayPendingCommissions();
        }
    }

    #region Helpers for RenderComissions
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

    /// <summary>
    /// Helper that instantiates or enables commission slots from pool to display active commissions.
    /// </summary>
    private void DisplayActiveCommissions()
    {
        DisplayCommissions(activeCommissions);
    }

    /// <summary>
    /// Helper that instantiates or enables commission slots from pool to display pending commissions.
    /// </summary>
    private void DisplayPendingCommissions()
    {
        DisplayCommissions(pendingCommissions);
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
