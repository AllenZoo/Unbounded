using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Attacker : MonoBehaviour
{
    [SerializeField] private List<EntityType> targetTypes = new List<EntityType>();
    [SerializeField] private GameObject attackObj;
    [SerializeField] private float cooldown = 0.5f;
    [SerializeField] private GameObject attackPool;

    // Variable that we will receive inputs to attack.
    private InputController inputController;
    private Attack attack;
    private bool attackRdy = true;

    private void Awake()
    {
        Assert.IsNotNull(attackObj.GetComponent<Rigidbody2D>(), "attack obj needs rb2d to set velocity");

        attack = attackObj.GetComponent<Attack>();
        Assert.IsNotNull(attack, "Attack Obj needs Attack component");

        inputController = GetComponentInParent<InputController>();
        Assert.IsNotNull(inputController, "Attacker needs InputController");

        attackPool = FindObjectOfType<AttackPool>().gameObject;
        Assert.IsNotNull(attackPool, "Attacker needs AttackPool");

        // Check if target types has atleast one element.
        Assert.IsTrue(targetTypes.Count > 0, "Attacker needs atleast one target type");
    }

    private void Start()
    {
        inputController.OnAttackInput += AttackReq;
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
        // Offset from attacker. TODO: make this a better calculation.
        float offset = 1f;

        Vector3 direction = info.mousePosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        Vector2 spawnPos = direction.normalized * offset + transform.position;
       
        // TODO: check if attackObj is in pool, use it. else, instantiate new one.
        // Spawn attack object a certain distance from attacker, rotated towards mouse.
        GameObject newAttackObj = attackPool.GetComponent<AttackPool>().GetAttack(attackObj);
        newAttackObj.transform.position = spawnPos;
        newAttackObj.transform.rotation = rotation;
        newAttackObj.SetActive(true);

        Attack newAttack = newAttackObj.GetComponent<Attack>();

        newAttack.ResetAttackAfterTime(newAttack.Duration);

        // Set velocity of attack (get from Attack in attackObj)
        newAttackObj.GetComponent<Rigidbody2D>().velocity = direction.normalized * attack.InitialSpeed;

        // Set valid EntityType targets for attack.
        newAttack.TargetTypes = targetTypes;
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
