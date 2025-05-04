using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Component attached to entities that attack!
/// </summary>
public class AttackerComponent : MonoBehaviour
{
    [Required, SerializeField]
    private Attacker attacker;

    [Required, SerializeField] private LocalEventHandler localEventHandler;

    [Tooltip("Types of entities this attacker can damage.")]
    [ValidateInput("ValidateList", "List cannot be empty!")]
    [Required, SerializeField] public List<EntityType> TargetTypes = new List<EntityType>();

    [Tooltip("Component that holds stats for adding damage to attacks.")]
    [SerializeField] private StatComponent statComponent;

    // Ugly code, but this is one way to implement it so lets do this for now.
    public double PercentageDamageIncrease { get; private set; } = 0;

    private bool attackRdy = true;
    private bool canAttack = true;

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

        LocalEventBinding<OnDeathEvent> deathEventBinding = new LocalEventBinding<OnDeathEvent>((e) => canAttack = false);
        localEventHandler.Register<OnDeathEvent>(deathEventBinding);

        LocalEventBinding<OnWeaponEquippedEvent> equipEventBinding = new LocalEventBinding<OnWeaponEquippedEvent>(HandleWeaponEquipped);
        localEventHandler.Register<OnWeaponEquippedEvent>(equipEventBinding);
    }

    public void AttackReq(OnAttackInput input)
    {
        // Attack if attack is ready and if data is not null.
        if (attackRdy && canAttack && attacker != null)
        {
            attacker.Attack(input.keyCode, input.attackInfo, this.transform, TargetTypes, statComponent.StatContainer.Attack, PercentageDamageIncrease);
            StartCoroutine(AttackCooldown());
        }
    }

    public void SetAttacker(Attacker attacker)
    {
        this.attacker = attacker;
    }

    private IEnumerator AttackCooldown()
    {
        attackRdy = false;
        yield return new WaitForSeconds(attacker.AttackerData.cooldown);
        attackRdy = true;
    }

    // Handles setting non-transform property of attacks..
    private IEnumerator ChargeUpAttack(Attack attack)
    {
        // Charge up attack
        yield return new WaitForSeconds(attacker.AttackerData.chargeUp);
    }
    private IEnumerator DeactivateAttack(GameObject attackObj, float duration)
    {
        yield return new WaitForSeconds(duration);
        // attackObj.GetComponent<Attack>().ResetAttack();
    }

    // Used for field validation via Odin.
    // Disabled since broken.
    private bool ValidateList(List<EntityType> value)
    {
        return value != null && value.Count > 0;
    }

    private void HandleWeaponEquipped(OnWeaponEquippedEvent e)
    {
        // Set the percentage increase.
        Item equipped = e.equipped;

        if (equipped != null)
        {
            PercentageDamageIncrease = equipped.ItemModifierMediator.GetPercentageDamageIncreaseTotal();
        }
    }
}
