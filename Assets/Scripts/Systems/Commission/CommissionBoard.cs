using Sirenix.OdinInspector;
using Sirenix.Reflection.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Handles managing commissions on the commission board.
/// TODO-OPT: refactor this responsibility somehwere else if this class gets too messy: 
///     Also handles the validation of commission submissions.
/// </summary>
public class CommissionBoard : MonoBehaviour
{

    [SerializeField]
    private float maxPendingCommissions = 3;

    [SerializeField]
    private float maxActiveCommissions = 1;

    [SerializeField]
    [Required]
    private SO_Inventory submitInventory;
    
    private List<Commission> commissions = new List<Commission>();
    private List<Tuple<Commission, Item>> completedCommissions = new List<Tuple<Commission, Item>>();
    private CommissionSubmissionValidator validator;

    private void Awake()
    {
        Assert.IsNotNull(submitInventory, "Submit Inventory is not set in the inspector.");
        validator = new CommissionSubmissionValidator();
    }

    private void Start()
    {
        Dictionary<Stat, int> stats = new Dictionary<Stat, int>();
        stats.Add(Stat.ATK, 1);
        stats.Add(Stat.DEF, 2);
        stats.Add(Stat.SPD, 3);

        // For testing purposes, add some commissions.
        Commission commission1 = new Commission("Commission1", "description", 1, 2, 2, EquipmentType.BOW, stats, CommissionStatus.ACTIVE);
        Commission commission2 = new Commission("Commission2", "description", 1, 2, 2, EquipmentType.BOW, stats, CommissionStatus.ACTIVE);
        Commission commission3 = new Commission("Commission3", "description", 1, 2, 2, EquipmentType.BOW, stats, CommissionStatus.PENDING);
        Commission commission4 = new Commission("Commission4", "description", 1, 2, 2, EquipmentType.BOW, stats, CommissionStatus.PENDING);


        Dictionary<Stat, int> stats2 = new Dictionary<Stat, int>();
        stats2.Add(Stat.ATK, 5);
        Commission commission5 = new Commission("Craft Sword", "Craft and Upgrade Katan's Sword!", 10, 1, 2, EquipmentType.SWORD, stats2, CommissionStatus.PENDING);

        //AddCommission(commission1);
        //AddCommission(commission2);
        AddCommission(commission3);
        AddCommission(commission4);
        AddCommission(commission5);
    }

    public List<Commission> GetActiveCommissions() => commissions.Where(commission => commission.commissionStatus.Equals(CommissionStatus.ACTIVE)).ToList();

    public List<Commission> GetPendingCommissions() => commissions.Where(commission => commission.commissionStatus.Equals(CommissionStatus.PENDING)).ToList();

    public void AddCommission(Commission commission)
    {
        if (commission.commissionStatus.Equals(CommissionStatus.ACTIVE) && GetActiveCommissions().Count >= maxActiveCommissions)
        {
            Debug.Log("Max active commissions reached.");
            return;
        } else if (commission.commissionStatus.Equals(CommissionStatus.PENDING) && commissions.Count >= maxPendingCommissions)
        {
            Debug.Log("Max pending commissions reached.");
            return;
        }

        commission.OnCommissionStart += AcceptCommission;
        commission.OnCommissionSubmitted += SubmitCommission;

        commissions.Add(commission);
        EventBus<OnCommissionListModifiedEvent>.Call(
            new OnCommissionListModifiedEvent { 
                activeCommissions = GetActiveCommissions(),
                pendingCommissions = GetPendingCommissions()
            }
        );
    }

    private void AcceptCommission(Commission commission)
    {
        // Redundent Check if the commission is in the list and also is
        // in pending status
        if (!commissions.Contains(commission) || commission.commissionStatus != CommissionStatus.PENDING)
        {
            Debug.LogError("Commission not found in list or not in pending status.");
            return;
        }

        // Check if max active commissions is reached
        if (GetActiveCommissions().Count >= maxActiveCommissions)
        {
            Debug.Log("Max active commissions reached.");
            return;
        }

        commission.commissionStatus = CommissionStatus.ACTIVE;
        EventBus<OnCommissionListModifiedEvent>.Call(
            new OnCommissionListModifiedEvent
            {
                activeCommissions = GetActiveCommissions(),
                pendingCommissions = GetPendingCommissions()
            }
        );
    }


    /// <summary>
    /// Validates the commission submission alongside the submitted item and then takes the appropriate action.
    /// </summary>
    /// <param name="commission"></param>
    private void SubmitCommission(Commission commission, Item submittedItem)
    {
        if (!validator.ValidateSubmission(commission, submittedItem))
        {
            // CompleteCommission(commission);
            Debug.Log("Submitted item does not meet the commission requirements.");
            return;
        }

        // Valid Submission. Move the item to completed commissions list.
        // Add money to Player's wallet.
        completedCommissions.Add(new Tuple<Commission, Item>(commission, submittedItem));
        submitInventory.ClearInventory();

        PlayerSingleton.Instance.GetComponentInChildren<StatComponent>().gold += commission.reward;

        HandleCommissionCompletion(commission);
    }

    private void HandleCommissionCompletion(Commission commission)
    {
        // Check if the commission is in list
        if (!commissions.Contains(commission))
        {
            Debug.Log("Commission not found in list.");
            return;
        }

        commission.commissionStatus = CommissionStatus.COMPLETED;
        EventBus<OnCommissionListModifiedEvent>.Call(
            new OnCommissionListModifiedEvent
            {
                activeCommissions = GetActiveCommissions(),
                pendingCommissions = GetPendingCommissions()
            }
        );

        commission.CompleteCommission();
    }
}
