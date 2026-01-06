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
    [Required, OdinSerialize]
    private IAttacker attacker;

    private IAttacker prevAttacker;


    [Required, SerializeField] private LocalEventHandler localEventHandler;

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

    [SerializeField, ReadOnly] private bool attackRdy = true;
    [SerializeField, ReadOnly] private bool canAttack = true;

    private Coroutine attackCDCoroutine;
    private Coroutine pauseCoroutine;

    private void Awake()
    {
        // Check if target types has atleast one element.
        Assert.IsTrue(TargetTypes.Count > 0, "Attacker needs atleast one target type");

        Assert.IsNotNull(statComponent, "Attacker needs stat component to get ATK value.");

        localEventHandler = InitializerUtil.FindComponentInParent<LocalEventHandler>(gameObject);
    }

    private void Start()
    {
        LocalEventBinding<OnAttackInput> eventBinding = new LocalEventBinding<OnAttackInput>(AttackReq);
        localEventHandler.Register<OnAttackInput>(eventBinding);

        LocalEventBinding<OnDeathEvent> deathEventBinding = new LocalEventBinding<OnDeathEvent>(
            (e) => { 
                canAttack = false;
                StopAllCoroutines();
            });
        localEventHandler.Register<OnDeathEvent>(deathEventBinding);

        LocalEventBinding<OnWeaponEquippedEvent> equipEventBinding = new LocalEventBinding<OnWeaponEquippedEvent>(HandleWeaponEquipped);
        localEventHandler.Register<OnWeaponEquippedEvent>(equipEventBinding);
    }

    public void AttackReq(OnAttackInput input)
    {
        // Attack if attack is ready and if data is not null.
        if (attackRdy && 
            canAttack && 
            attacker != null && 
            attacker.IsInitialized() && 
            attacker.CanAttack())
        {
            // Stop previous attack
            attacker.StopAttack();

            // Create new attack context.
            AttackContext ac = new AttackContext(
                input.attackInfo,
                this,
                this.transform,
                TargetTypes,
                statComponent.StatContainer.Attack,
                PercentageDamageIncrease
            );

            // Start Attack
            attacker.Attack(input.keyCode, ac);
            attackCDCoroutine = StartCoroutine(AttackCooldown());
        }
    }

    public void SetAttacker(IAttacker attacker)
    {
        // Kill any previous ongoing attacks
        if (this.attacker != null && this.attacker != attacker)
        {
            this.attacker.StopAttack();
        }


        // Kill any ongoing cooldown coroutine
        if (attackCDCoroutine != null)
        {
            StopCoroutine(attackCDCoroutine);
            attackRdy = true;
        }

        this.attacker = attacker;
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

    private IEnumerator DeactivateAttacker(float duration)
    {
        canAttack = false;
        yield return new WaitForSeconds(duration);
        canAttack = true;
    }
    private IEnumerator AttackCooldown()
    {
        attackRdy = false;
        var attackCd = GetCooldownExponential(attacker.GetCooldown(), statComponent.StatContainer.Dexterity, 0.05f);
        yield return new WaitForSeconds(attackCd);
        attackRdy = true;
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
