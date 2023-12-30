using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Attacker : MonoBehaviour
{
    [SerializeField] private GameObject attackObj;

    private void Awake()
    {
        Assert.IsNotNull(attackObj.GetComponent<Rigidbody2D>(), "attack obj needs rb2d to set velocity");
    }

    public void Attack(Vector2 pos, Quaternion rot, Vector2 velocity, Vector2 dir)
    {
        GameObject attack = Instantiate(attackObj, transform.position, Quaternion.identity);
        // Set velocity of attack
        attack.GetComponent<Rigidbody2D>().velocity = velocity;
    }

    
}
