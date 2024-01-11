using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Attacker : MonoBehaviour
{
    [SerializeField] public List<EntityType> TargetTypes = new List<EntityType>();

    [SerializeField] private GameObject attackObj;

    [Tooltip("Number of attacks to spawn when attacking.")]
    [SerializeField] private int numAttacks = 1;

    [Tooltip("Angle offset for between each attack spawned.")]
    [SerializeField] private float angleOffset = 0f;

    // Could consider moving this to Attack, but it might make sense for the attacker to control the 
    // cooldown of attacks.
    [SerializeField] private float cooldown = 0.5f;

    [Tooltip("Input controller to receive inputs to attack from.")]
    [SerializeField] private InputController inputController;

    private Attack attack;
    private bool attackRdy = true;

    private void Awake()
    {
        Assert.IsNotNull(attackObj.GetComponent<Rigidbody2D>(), "attack obj needs rb2d to set velocity");

        attack = attackObj.GetComponent<Attack>();
        Assert.IsNotNull(attack, "Attack Obj needs Attack component");

        if (inputController == null)
        {
            inputController = GetComponentInParent<InputController>();
        }
        // Assert.IsNotNull(inputController, "Attacker needs InputController");

        // Check if target types has atleast one element.
        Assert.IsTrue(TargetTypes.Count > 0, "Attacker needs atleast one target type");
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


    public void SetAttacker(SO_Attacker data)
    {
        TargetTypes = data.TargetTypes;
        attackObj = data.attackObj;
        numAttacks = data.numAttacks;
        angleOffset = data.angleOffset;
        cooldown = data.cooldown;

        SetAttackObj(attackObj);
    }

    // Reset all fields related to attack.
    public void SetAttackObj(GameObject attackObj)
    {
        if (attackObj == null)
        {
            // TODO: set default attack object or disable attacker.
            return;
        }
        this.attackObj = attackObj;
        this.attack = attackObj.GetComponent<Attack>();
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
        for (int i = 0; i < numAttacks; i++)
        {
            // i = 0, shoot torwards mouse.
            // i = 1, shoot to the right of mouse with angleOffset * 1 away from attack 0.
            // i = 2, shoot to the left of mouse with angleOffset * 1 away from attack 0.
            //float angle = (i+1)/2 * angleOffset;
            float angle = (int) ((i+1)/2) * angleOffset;

            // Odd's offset to the right.
            // Even's offset to the left
            if (i % 2 == 1)
            {
                // odd
                angle *= -1;
            }

            Vector3 attackDir = Quaternion.Euler(0, 0, angle) * (info.mousePosition - transform.position);

            AttackSpawner.SpawnAttack(attackDir, transform, TargetTypes, attackObj, attack);
        }
    }

    // Handles setting non-transform property of attacks..
    public IEnumerator ChargeUpAttack(Attack attack)
    {
        // Charge up attack
        yield return new WaitForSeconds(attack.ChargeUp);
    }

    public IEnumerator AttackCooldown()
    {
        attackRdy = false;
        yield return new WaitForSeconds(cooldown);
        attackRdy = true;
    }

    public IEnumerator DeactivateAttack(GameObject attackObj, float duration)
    {
        yield return new WaitForSeconds(duration);
        attackObj.GetComponent<Attack>().ResetAttack();
    }

    
}
