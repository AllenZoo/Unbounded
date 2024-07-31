using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data class that holds the information of a commission/quest.
/// </summary>
public class Commission
{ 
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
}

public enum CommissionStatus
{
    ACTIVE,
    PENDING,
    COMPLETED,
    FAILED
}
