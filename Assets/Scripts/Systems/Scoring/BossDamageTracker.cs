using UnityEngine;

/// <summary>
/// Component attached to boss entities to track damage taken for scoring purposes.
/// Listens to OnDamagedEvent and reports to RunTracker.
/// </summary>
[RequireComponent(typeof(Damageable))]
[RequireComponent(typeof(LocalEventHandler))]
public class BossDamageTracker : MonoBehaviour
{
    private Damageable damageable;
    private LocalEventHandler leh;
    private bool hasRegistered = false;

    // Store event bindings for proper cleanup
    private LocalEventBinding<OnDamagedEvent> damageBinding;
    private LocalEventBinding<OnDeathEvent> deathBinding;

    private void Awake()
    {
        damageable = GetComponent<Damageable>();
        leh = GetComponent<LocalEventHandler>();
        
        // Verify this is actually a boss entity
        if (damageable.EntityType != EntityType.BOSS_MONSTER)
        {
            Debug.LogWarning($"BossDamageTracker on {gameObject.name} is not attached to a BOSS_MONSTER entity type.");
            return;
        }
    }

    private void Start()
    {
        // Register to listen for damage events and store bindings for cleanup
        damageBinding = new LocalEventBinding<OnDamagedEvent>(OnBossDamaged);
        leh.Register(damageBinding);

        // Register to listen for death events to end boss fight
        deathBinding = new LocalEventBinding<OnDeathEvent>(OnBossDeath);
        leh.Register(deathBinding);
        
        hasRegistered = true;
    }

    private void OnDestroy()
    {
        // Unregister event bindings to prevent memory leaks
        if (hasRegistered && leh != null)
        {
            if (damageBinding != null)
            {
                leh.Unregister(damageBinding);
            }
            
            if (deathBinding != null)
            {
                leh.Unregister(deathBinding);
            }
        }
    }

    /// <summary>
    /// Called when the boss takes damage. Reports to RunTracker.
    /// </summary>
    private void OnBossDamaged(OnDamagedEvent e)
    {
        if (RunTracker.Instance != null)
        {
            RunTracker.Instance.RecordDamageDealt(e.damage, damageable.EntityType);
        }
    }

    /// <summary>
    /// Called when the boss dies. Triggers end of boss fight.
    /// </summary>
    private void OnBossDeath(OnDeathEvent e)
    {
        if (RunTracker.Instance != null)
        {
            // Fire the boss fight end event
            EventBus<OnBossFightEndEvent>.Call(new OnBossFightEndEvent { bossName = gameObject.name });
        }
    }
}
