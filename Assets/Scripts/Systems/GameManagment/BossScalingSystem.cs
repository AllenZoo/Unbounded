using UnityEngine;

/// <summary>
/// System that scales boss stats based on the current round number.
/// Applies scaling modifiers to boss StatComponents on initialization.
/// Note: Scaling is applied once during Awake(). Should not be called multiple times.
/// </summary>
public class BossScalingSystem : MonoBehaviour
{
    [SerializeField] private StatComponent statComponent;
    [SerializeField] private Damageable damageable;

    [Header("Scaling Configuration")]
    [Tooltip("Base health multiplier per round. E.g., 1.15 = 15% increase per round")]
    [SerializeField] private float healthScalingMultiplier = 1.15f;

    [Tooltip("Base attack multiplier per round. E.g., 1.10 = 10% increase per round")]
    [SerializeField] private float attackScalingMultiplier = 1.10f;

    [Tooltip("Base defense multiplier per round. E.g., 1.08 = 8% increase per round")]
    [SerializeField] private float defenseScalingMultiplier = 1.08f;

    [Tooltip("Maximum round number for scaling (default: 10)")]
    [SerializeField] private int maxRoundNumber = 10;

    // Flag to prevent double-scaling if Awake is called multiple times
    private bool hasAppliedScaling = false;

    private void Awake()
    {
        // Ensure we have required components
        if (statComponent == null)
        {
            statComponent = GetComponent<StatComponent>();
        }

        if (damageable == null)
        {
            damageable = GetComponent<Damageable>();
        }

        // Only apply scaling if this is a boss entity
        if (damageable != null && damageable.EntityType == EntityType.BOSS_MONSTER)
        {
            ApplyBossScaling();
        }
    }

    /// <summary>
    /// Applies scaling modifiers to boss stats based on current round number.
    /// </summary>
    private void ApplyBossScaling()
    {
        // Prevent double-scaling
        if (hasAppliedScaling)
        {
            Debug.LogWarning($"BossScalingSystem on {gameObject.name}: Scaling already applied, skipping.");
            return;
        }

        if (statComponent == null)
        {
            Debug.LogWarning($"BossScalingSystem on {gameObject.name}: StatComponent is null, cannot apply scaling.");
            return;
        }

        // Get current round number from GameManager
        int currentRound = GameManagerComponent.Instance?.roundNumber ?? 1;

        // Clamp to max round for balance purposes
        currentRound = Mathf.Min(currentRound, maxRoundNumber);

        // Calculate scaling factors (rounds start at 1, so we scale from round 2 onwards)
        // effectiveRounds represents the number of rounds beyond the first
        float effectiveRounds = currentRound - 1; // Round 1 = 0, Round 2 = 1, etc.

        // Calculate stat increases using compound growth formula
        float healthIncrease = CalculateStatIncrease(statComponent.StatContainer.MaxHealth, healthScalingMultiplier, effectiveRounds);
        float attackIncrease = CalculateStatIncrease(statComponent.StatContainer.Attack, attackScalingMultiplier, effectiveRounds);
        float defenseIncrease = CalculateStatIncrease(statComponent.StatContainer.Defense, defenseScalingMultiplier, effectiveRounds);

        // Apply modifiers through the stat system
        if (healthIncrease > 0)
        {
            StatModifier healthModifier = new StatModifier(Stat.MAX_HP, new AddOperation(healthIncrease), -1);
            statComponent.StatContainer.StatMediator.AddModifier(this, healthModifier);

            // Increase current health proportionally if boss is at full health
            // Otherwise preserve the damage already taken
            float currentHealthPercent = statComponent.StatContainer.Health / (statComponent.StatContainer.MaxHealth - healthIncrease);
            if (currentHealthPercent >= 0.99f) // Close to full health
            {
                statComponent.StatContainer.Health = statComponent.StatContainer.MaxHealth;
            }
            else
            {
                // Scale current health proportionally to maintain damage taken percentage
                statComponent.StatContainer.Health = statComponent.StatContainer.MaxHealth * currentHealthPercent;
            }
        }

        if (attackIncrease > 0)
        {
            StatModifier attackModifier = new StatModifier(Stat.ATK, new AddOperation(attackIncrease), -1);
            statComponent.StatContainer.StatMediator.AddModifier(this, attackModifier);
        }

        if (defenseIncrease > 0)
        {
            StatModifier defenseModifier = new StatModifier(Stat.DEF, new AddOperation(defenseIncrease), -1);
            statComponent.StatContainer.StatMediator.AddModifier(this, defenseModifier);
        }

        // Mark as scaled
        hasAppliedScaling = true;

        if (Debug.isDebugBuild)
        {
            Debug.Log($"Boss Scaling Applied for Round {currentRound}:");
            Debug.Log($"  Health: +{healthIncrease:F1} (Total: {statComponent.StatContainer.MaxHealth:F1})");
            Debug.Log($"  Attack: +{attackIncrease:F1} (Total: {statComponent.StatContainer.Attack:F1})");
            Debug.Log($"  Defense: +{defenseIncrease:F1} (Total: {statComponent.StatContainer.Defense:F1})");
        }
    }

    /// <summary>
    /// Calculates the stat increase using compound growth formula.
    /// </summary>
    /// <param name="baseStat">Base stat value</param>
    /// <param name="multiplier">Multiplier per round (e.g., 1.15 for 15% increase)</param>
    /// <param name="rounds">Number of rounds to scale for</param>
    /// <returns>Additional stat value to add</returns>
    private float CalculateStatIncrease(float baseStat, float multiplier, float rounds)
    {
        if (rounds <= 0) return 0;

        // Compound growth: newValue = baseStat * (multiplier ^ rounds)
        // Increase = newValue - baseStat
        float scaledValue = baseStat * Mathf.Pow(multiplier, rounds);
        float increase = scaledValue - baseStat;

        return increase;
    }
}
