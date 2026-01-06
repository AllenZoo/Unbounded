using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Component attached to entities that attack!
/// </summary>
public class AttackerComponent : SerializedMonoBehaviour
{
    //[Required, OdinSerialize]
    //private IAttacker attacker;

    [Required, OdinSerialize]
    private List<AttackSlot> attackSlots = new();

    [Required, SerializeField] private LocalEventHandler leh;

    [Tooltip("Types of entities this attacker can damage.")]
    [ValidateInput(nameof(ValidateList), "List cannot be empty!")]
    [Required, SerializeField] public List<EntityType> TargetTypes = new List<EntityType>();
    #region Validate List
    // Used for field validation via Odin.
    private bool ValidateList(List<EntityType> value)
    {
        return value != null && value.Count > 0;
    }
    #endregion

    [Tooltip("Component that holds stats for adding damage to attacks.")]
    [SerializeField] private StatComponent statComponent;

    // Ugly code, but this is one way to implement it so lets do this for now.
    public double PercentageDamageIncrease { get; private set; } = 0;

    // Whether the attacker component can attack.
    [SerializeField, ReadOnly] private bool canAttack = true;
    private Coroutine pauseCoroutine;

    private void Awake()
    {
        // Check if target types has atleast one element.
        Assert.IsTrue(TargetTypes.Count > 0, "Attacker needs atleast one target type");

        Assert.IsNotNull(statComponent, "Attacker needs stat component to get ATK value.");

        leh = InitializerUtil.FindComponentInParent<LocalEventHandler>(gameObject);
    }

    private void Start()
    {
        LocalEventBinding<OnAttackInput> eventBinding = new LocalEventBinding<OnAttackInput>(AttackReq);
        leh.Register<OnAttackInput>(eventBinding);

        LocalEventBinding<OnDeathEvent> deathEventBinding = new LocalEventBinding<OnDeathEvent>(
            (e) => { 
                canAttack = false;
                StopAllCoroutines();
            });
        leh.Register<OnDeathEvent>(deathEventBinding);

        LocalEventBinding<OnWeaponEquippedEvent> equipEventBinding = new LocalEventBinding<OnWeaponEquippedEvent>(HandleWeaponEquipped);
        leh.Register<OnWeaponEquippedEvent>(equipEventBinding);
    }

    public void AttackReq(OnAttackInput input)
    {
        // Attack if attack is ready and if data is not null.
        if (canAttack)
        {
            // Create new attack context.
            AttackContext ac = new AttackContext(
                input.attackInfo,
                this,
                this.transform,
                TargetTypes,
                statComponent.StatContainer.Attack,
                PercentageDamageIncrease
            );

            foreach (var attackSlot in attackSlots)
            {
                // Check if ready. E.g. if cooldown has recovered.
                if (attackSlot.IsReady)
                {
                    // Note: Cooldown is handled in attack slot
                    attackSlot.Trigger(input.keyCode, ac);
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="attackers">attackers to add</param>
    /// <param name="reset">whether to accumulate the given attacker with old list or to erase old attack slots</param>
    public void SetAttacker(List<IAttacker> attackers, bool reset = true)
    {
        if (reset)
        {
            // Kill any previous ongoing attacks
            KillAttackSlots();
        }

        if (attackers == null || attackers.Count <= 0) return;

        foreach (var attacker in attackers)
        {
            AttackSlot slot = new AttackSlot(attacker);
            attackSlots.Add(slot);
        }
    }

    private void KillAttackSlots()
    {
        foreach (var attackSlot in attackSlots)
        {
            attackSlot.StopTrigger();
        }
        attackSlots.Clear();
    }


    /// <summary>
    /// Pauses any attacks for a given duration.
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    public void PauseAttacker(float duration)
    {
        if (pauseCoroutine  != null)
        {
            StopCoroutine(pauseCoroutine);
        }
        pauseCoroutine = StartCoroutine(DeactivateAttacker(duration));
    }

    /// <summary>
    /// Deactivates Attacker Component for a set duration.
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    private IEnumerator DeactivateAttacker(float duration)
    {
        canAttack = false;
        yield return new WaitForSeconds(duration);
        canAttack = true;
    }

    private void HandleWeaponEquipped(OnWeaponEquippedEvent e)
    {
        // Set the percentage increase.
        Item equipped = e.equipped;
        Item unequipped = e.unequipped;

        if (unequipped != null && !unequipped.IsEmpty())
        {
            unequipped.ItemModifierMediator.OnModifierChange -= UpdatePercentageDamageIncrease;
        }

        if (equipped != null && !equipped.IsEmpty())
        {
            UpdatePercentageDamageIncrease(equipped);
            equipped.ItemModifierMediator.OnModifierChange += UpdatePercentageDamageIncrease;
        }
    }

    private void UpdatePercentageDamageIncrease(Item weapon)
    {
        if (weapon != null)
        {
            PercentageDamageIncrease = weapon.ItemModifierMediator.QueryPercentageDamageIncreaseTotal();
        }
    }

    float GetCooldownExponential(float baseCooldown, float dex, float scalingFactor = 0.05f, float minCooldown = 0.01f)
    {
        return Mathf.Max(minCooldown, baseCooldown / (1f + (dex * scalingFactor)));
    }

    float GetCooldownLinear(float baseCooldown, float dex, float dexMultiplier = 0.01f)
    {
        return Mathf.Max(0.05f, baseCooldown - (dex * dexMultiplier));
    }

}
