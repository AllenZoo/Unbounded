using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data class that holds the information of a commission/quest.
/// </summary>
public class Commission
{
    public Action<Commission> OnCommissionStart;
    public Action<Commission, Item> OnCommissionSubmitted;
    public Action<Commission> OnCommissionComplete;

    public string title { get; private set; }
    public string description { get; private set; }
    public int reward { get; private set; }
    public int difficulty { get; private set; }
    public int timeLimit { get; private set; }
    public EquipmentType equipmentType { get; private set; }
    public Dictionary<Stat, int> statRequirements { get; private set; }
    public CommissionStatus commissionStatus;

    public Commission(string title, string description, int reward, int difficulty, int timeLimit, EquipmentType equipmentType, Dictionary<Stat, int> statRequirements, CommissionStatus commissionStatus)
    {
        this.title = title;
        this.description = description;
        this.reward = reward;
        this.difficulty = difficulty;
        this.timeLimit = timeLimit;
        this.equipmentType = equipmentType;
        this.statRequirements = statRequirements;
        this.commissionStatus = commissionStatus;
    }

    /// <summary>
    /// Attempts to start the commission. If the commission is not pending, it will not start.
    /// </summary>
    public void StartCommission()
    {
        if (commissionStatus != CommissionStatus.PENDING)
        {
            Debug.Log("Commission is not pending. Cannot start it.");
            return;
        }

        OnCommissionStart?.Invoke(this);
    }

    /// <summary>
    /// Attempts to complete the commission. If the commission is not active, it will not complete.
    /// </summary>
    public void SubmitCommission(Item item)
    {
        if (commissionStatus != CommissionStatus.ACTIVE)
        {
            Debug.Log("Commission is not active. Cannot submit it.");
            return;
        }
        OnCommissionSubmitted?.Invoke(this, item);
    }

    /// <summary>
    /// Callback for when the commission is completed.
    /// </summary>
    public void CompleteCommission()
    {
        if (commissionStatus != CommissionStatus.COMPLETED)
        {
            Debug.Log("Commission is not complete.");
            return;
        }

        OnCommissionComplete?.Invoke(this);
    }
}

public enum CommissionStatus
{
    ACTIVE,
    PENDING,
    COMPLETED,
    FAILED
}
