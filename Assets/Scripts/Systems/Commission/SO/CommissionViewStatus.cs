using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CommissionViewStatus", menuName = "ScriptableObjects/CommissionViewStatus", order = 1)]
public class CommissionViewStatus : ScriptableObject
{
    public Action<CommissionStatusChangedEvent> OnStatusChanged;

    [SerializeField] private CommissionViewStatusType status;

    public void SetStatus(CommissionViewStatusType newStatus)
    {
        CommissionStatusChangedEvent statusChangedEvent = new CommissionStatusChangedEvent
        {
            prevStatus = status,
            newStatus = newStatus
        };

        status = newStatus;
        OnStatusChanged?.Invoke(statusChangedEvent);
    }

    public CommissionViewStatusType GetStatus()
    {
        return status;
    }
}

public enum CommissionViewStatusType
{
    ACTIVE,
    PENDING,
}

public struct CommissionStatusChangedEvent
{
    public CommissionViewStatusType prevStatus;
    public CommissionViewStatusType newStatus;
}