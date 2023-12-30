using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Attacker : MonoBehaviour
{
    [SerializeField] private GameObject attackObj;

    // Variable that we will receive inputs to attack.
    private InputController inputController;
    private Attack attack;

    private void Awake()
    {
        Assert.IsNotNull(attackObj.GetComponent<Rigidbody2D>(), "attack obj needs rb2d to set velocity");

        attack = attackObj.GetComponent<Attack>();
        Assert.IsNotNull(attack, "Attack Obj needs Attack component");

        inputController = GetComponentInParent<InputController>();
        Assert.IsNotNull(inputController, "Attacker needs InputController");
    }

    private void Start()
    {
        inputController.OnAttackInput += Attack;
    }

    public void Attack(KeyCode keyCode, AttackSpawnInfo info)
    {
        // Offset from attacker. TODO: make this a better calculation.
        float offset = 1f;

        // Calculate the direction from the attacker to the mouse
        Vector3 direction = info.mousePosition - transform.position;

        // Calculate the rotation angle in radians using Mathf.Atan2
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Create a rotation quaternion based on the angle
        Quaternion rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));

        // Spawn position of attack
        Vector2 spawnPos = direction.normalized * offset + transform.position;
       
        // Spawn attack object a certain distance from attacker, rotated towards mouse.
        GameObject newAttackObj = Instantiate(attackObj, spawnPos, rotation);

        // Delete newAttackObj after attack.Duration
        // TODO: optimize by using an attack pool.
        Destroy(newAttackObj, attack.Duration);


        // TODO:  Set velocity of attack (get from Attack in attackObj)
        // attack.GetComponent<Rigidbody2D>().velocity = 0;
    }


}
