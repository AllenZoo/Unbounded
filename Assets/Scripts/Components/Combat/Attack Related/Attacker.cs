using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Attacker : MonoBehaviour
{
    [SerializeField] public List<EntityType> TargetTypes = new List<EntityType>();

    // Could be unecessary. But useful for quickly serializing values.
    [Tooltip("To init attacker data. Leave empty if we want to manually set attacker data values.")]
    [SerializeField] private SO_Attacker attackerDataInit;

    [SerializeField] private AttackerData data;

    [Tooltip("Input controller to receive inputs to attack from.")]
    [SerializeField] private InputController inputController;

    [Tooltip("Component that holds stats for adding damage to attacks.")]
    [SerializeField] private StatComponent statComponent;

    private bool attackRdy = true;

    private void Awake()
    {
        if (inputController == null)
        {
            Debug.LogWarning("Attacker input controller is null. " +
                "Attempting to get input controller from parent.");
            inputController = GetComponentInParent<InputController>();
        }

        // Check if target types has atleast one element.
        Assert.IsTrue(TargetTypes.Count > 0, "Attacker needs atleast one target type");

        Assert.IsNotNull(statComponent, "Attacker needs stat component to get ATK value.");

        if (attackerDataInit != null)
        {
            // Debug.Log("Attacker data init is not null. Setting attacker data to attacker data init.");
            // Init. Avoid pass by ref.
            SetAttacker(attackerDataInit);
        }
    }

    private void Start()
    {
        if (inputController != null)
        {
            inputController.OnAttackInput += AttackReq;
        } else
        {
            Debug.LogWarning("Attacker has no input controller! This means that attacker will never attack as there is no input" +
                "for it to listen to that calls it to attack.");
        }
    }

    public void SetAttacker(SO_Attacker newData)
    {

        if (newData == null)
        {
            Debug.LogWarning("Attacker data is null. Cannot set attacker data.");
            return;
        }

        data = newData.data.Copy();
    }

    public void AttackReq(KeyCode keyCode, AttackSpawnInfo info)
    {
        if (attackRdy)
        {
            Attack(keyCode, info);
            StartCoroutine(AttackCooldown());
        }
    }

    public void Attack(KeyCode keyCode, AttackSpawnInfo info)
    {
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
