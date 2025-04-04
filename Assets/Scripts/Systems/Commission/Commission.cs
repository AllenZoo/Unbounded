using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data class that holds the information of a commission/quest.
/// </summary>
[Serializable]
public class Commission
{
    public Action<Commission> OnCommissionStart;
    public Action<Commission, Item> OnCommissionSubmitted;
    public Action<Commission> OnCommissionComplete;

    [ShowInInspector] public string title { get; private set; }
    [ShowInInspector] public string description { get; private set; }
    [ShowInInspector] public int reward { get; private set; }
    [ShowInInspector] public int difficulty { get; private set; }
    [ShowInInspector] public int timeLimit { get; private set; }
    [ShowInInspector] public EquipmentType equipmentType { get; private set; }
    [ShowInInspector] public Dictionary<Stat, int> statRequirements { get; private set; }
    [ShowInInspector] public CommissionStatus commissionStatus;
    [ShowInInspector] public Sprite itemImage { get; private set; }
    [ShowInInspector] public float rotOffset { get; private set; }
    

    public Commission(string title, string description, int reward, int difficulty, int timeLimit, EquipmentType equipmentType, Dictionary<Stat, int> statRequirements, CommissionStatus commissionStatus, Sprite itemImage, float rotOffset)
    {
        this.title = title;
        this.description = description;
        this.reward = reward;
        this.difficulty = difficulty;
        this.timeLimit = timeLimit;
        this.equipmentType = equipmentType;
        this.statRequirements = statRequirements;
        this.commissionStatus = commissionStatus;
        this.itemImage = itemImage;
        this.rotOffset = rotOffset;
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
