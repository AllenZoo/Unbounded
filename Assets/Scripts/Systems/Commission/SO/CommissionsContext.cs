using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that acts as the global state holder of current commissions and accepted commission.
/// 
/// With how current commission system is going to work, we allow the player to select 1 commission from 3 generated choices,
/// and then after they complete that commission, we repeat.
/// </summary>
[CreateAssetMenu(fileName ="new commission context", menuName = "System/Commission/CommissionContext")]
public class CommissionsContext : ScriptableObject
{
    public Action OnCommissionContextChange;
    [SerializeField, ReadOnly] public List<Commission> Commissions { get { return Commissions; } set { Commissions = value; OnCommissionContextChange?.Invoke(); } }
    [SerializeField, ReadOnly] public Commission AcceptedCommission { get { return AcceptedCommission; } set { AcceptedCommission = value; OnCommissionContextChange?.Invoke(); } }
}
