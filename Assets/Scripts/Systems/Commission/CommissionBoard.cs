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

    public List<Commission> GetActiveCommissions() => commissions.Where(commission => commission.commissionStatus.Equals(CommissionStatus.ACTIVE)).ToList();

    public List<Commission> GetPendingCommissions() => commissions.Where(commission => commission.commissionStatus.Equals(CommissionStatus.PENDING)).ToList();

    public void AddCommission(Commission commission)
    {
        if (commissions.Count >= maxPendingCommissions)
        {
            Debug.Log("Max pending commissions reached.");
            return;
        }
        commissions.Add(commission);
        EventBus<OnCommissionListModifiedEvent>.Call(
            new OnCommissionListModifiedEvent { 
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
