using Sirenix.Reflection.Editor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Handles managing commissions.
/// </summary>
public class CommissionBoard : MonoBehaviour
{

    [SerializeField]
    private float maxPendingCommissions = 3;

    [SerializeField]
    private float maxActiveCommissions = 1;

    private List<Commission> commissions = new List<Commission>();

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

        //AddCommission(commission1);
        //AddCommission(commission2);
        AddCommission(commission3);
        AddCommission(commission4);
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

        commissions.Add(commission);
        EventBus<OnCommissionListModifiedEvent>.Call(
            new OnCommissionListModifiedEvent { 
                activeCommissions = GetActiveCommissions(),
                pendingCommissions = GetPendingCommissions()
            }
        );
    }

    public void AcceptCommission(Commission commission)
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

    public void CompleteCommission(Commission commission)
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
    }
}
