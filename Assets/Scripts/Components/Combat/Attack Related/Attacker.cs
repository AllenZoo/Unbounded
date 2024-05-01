using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Assertions;

public class Attacker : MonoBehaviour
{
    [NotNull]
    [SerializeField] private LocalEventHandler localEventHandler;

    [Tooltip("Types of entities this attacker can damage.")]
    [SerializeField] public List<EntityType> TargetTypes = new List<EntityType>();

    // Could be unecessary. But useful for quickly serializing values.
    [Tooltip("To init attacker data. Leave empty if we want to manually set attacker data values.")]
    [SerializeField] private SO_Attacker attackerDataInit;

    [SerializeField] private AttackerData data;

    [Tooltip("Component that holds stats for adding damage to attacks.")]
    [SerializeField] private StatComponent statComponent;

    private bool attackRdy = true;

    private void Awake()
    {
        // Check if target types has atleast one element.
        Assert.IsTrue(TargetTypes.Count > 0, "Attacker needs atleast one target type");

        Assert.IsNotNull(statComponent, "Attacker needs stat component to get ATK value.");

        if (attackerDataInit != null)
        {
            // Debug.Log("Attacker data init is not null. Setting attacker data to attacker data init.");
            // Init. Avoid pass by ref.
            SetAttacker(attackerDataInit);
        }

        if (localEventHandler == null)
        {
            localEventHandler = GetComponentInParent<LocalEventHandler>();
            if (localEventHandler == null)
            {
                Debug.LogError("LocalEventHandler unassgined and not found in parent for object [" + gameObject +
                    "] with root object [" + gameObject.transform.root.name + "]");
            }
        }
    }

    private void Start()
    {
        LocalEventBinding<OnAttackInput> eventBinding = new LocalEventBinding<OnAttackInput>(AttackReq);
        localEventHandler.Register<OnAttackInput>(eventBinding);
    }

    public void SetAttacker(SO_Attacker newData)
    {
        // Debug.Log("Set Attacker: " + newData);
        if (newData == null)
        {
            Debug.LogWarning("Attacker data is null. Cannot attack.");
            data = null;
            return;
        }

        data = newData.data.Copy();
    }

    public void AttackReq(OnAttackInput input)
    {
        // Attack if attack is ready and if data is not null.
        if (attackRdy && data != null)
        {
            Attack(input.keyCode, input.attackInfo);
        }
    }

    // Attacks and starts cooldown at end of attack. If data or data.attackObj is null, then this function
    // does nothing.
    public void Attack(KeyCode keyCode, AttackSpawnInfo info)
    {
        if (data == null || data.attackObj == null)
        {
            return;
        }

        for (int i = 0; i < data.numAttacks; i++)
        {
            // i = 0, shoot torwards mouse.
            // i = 1, shoot to the right of mouse with angleOffset * 1 away from attack 0.
            // i = 2, shoot to the left of mouse with angleOffset * 1 away from attack 0.
            //float angle = (i+1)/2 * angleOffset;
            float angle = (int) ((i+1)/2) * data.angleOffset;

            // Odd's offset to the right.
            // Even's offset to the left
            if (i % 2 == 1)
            {
                // odd
                angle *= -1;
            }

            Vector3 attackDir = Quaternion.Euler(0, 0, angle) * (info.mousePosition - transform.position);

            Attack newAttack = AttackSpawner.SpawnAttack(attackDir, transform, TargetTypes, data.attackObj);
            newAttack.attackerATKStat = statComponent.GetCurStat(Stat.ATK);
        }

        StartCoroutine(AttackCooldown());
    }

    // Handles setting non-transform property of attacks..
    public IEnumerator ChargeUpAttack(Attack attack)
    {
        // Charge up attack
        yield return new WaitForSeconds(attack.Data.chargeUp);
    }

    public IEnumerator AttackCooldown()
    {
        attackRdy = false;
        yield return new WaitForSeconds(data.cooldown);
        attackRdy = true;
    }

    public IEnumerator DeactivateAttack(GameObject attackObj, float duration)
    {
        yield return new WaitForSeconds(duration);
        attackObj.GetComponent<Attack>().ResetAttack();
    }
}
