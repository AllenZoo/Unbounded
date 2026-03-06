using UnityEngine;
using System;

[CreateAssetMenu(fileName = "new damage target condition", menuName = "System/Objective/Conditions/DamageTargetCondition")]
public class DamageTargetConditionData : ObjectiveConditionData
{
    [Tooltip("Damage necessary to cross threshold, and complete condition.")]
    public float damageAmountThreshold = 1f;

    [Tooltip("Target that we need to damage for this condition to be satisfied.")]
    public DamageableContext damageableContext;

    public override IObjectiveCondition CreateInstance()
    {
        return new DamageTargetCondition(this);
    }
}

public class DamageTargetCondition : IObjectiveCondition
{
    public event Action OnStateChanged;

    private DamageTargetConditionData data;
    private Objective owner;
    private float accumulatedDamage = 0f;

    private Damageable damageableComponent;

    public DamageTargetCondition(DamageTargetConditionData data)
    {
        this.data = data;
        this.accumulatedDamage = 0;
    }
    
    /// <summary>
    /// Determines whether the accumulated damage meets or exceeds the configured damage threshold.
    /// </summary>
    /// <returns>true if the accumulated damage is greater than or equal to the damage amount threshold; otherwise, false.</returns>
    public bool IsMet() => accumulatedDamage >= data.damageAmountThreshold;
    public void Initialize(Objective owner)
    {
        this.owner = owner;
        damageableComponent = data.damageableContext.GetContext().Value;
        if (damageableComponent == null)
        {
            // Context not loaded yet, subscribe to context change event, which will fire when it does load.
            data.damageableContext.OnContextChanged += HandleDamageableContextChanged;
        } else
        {
            damageableComponent.OnDamaged += HandleDamage;
        }
    }
    public void Cleanup() {

        if (damageableComponent != null)
        {
            damageableComponent.OnDamaged -= HandleDamage;
        } else
        {
            Debug.LogError("Tried to cleanup null Damageable Component! (Shouldn't really happen.)");
        }

        data.damageableContext.OnContextChanged -= HandleDamageableContextChanged;
    }

    public void Update(float deltaTime) { }

    private void HandleDamage(float damageAmount)
    {
        if (owner.State != ObjectiveState.ACTIVE) return;

        accumulatedDamage += damageAmount;
        if (accumulatedDamage >= data.damageAmountThreshold)
        {
            OnStateChanged?.Invoke();
        }
    }

    private void HandleDamageableContextChanged(Damageable newDamageable)
    {
        if (damageableComponent != null)
        {
            damageableComponent.OnDamaged -= HandleDamage;
        }
        damageableComponent = newDamageable;
        if (damageableComponent != null)
        {
            damageableComponent.OnDamaged += HandleDamage;
        }
        else
        {
            Debug.LogError("DamageTargetCondition: No valid Damageable found in context.");
        }
    }
}
