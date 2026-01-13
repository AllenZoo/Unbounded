using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// TODO: refactor this an attack script to use scriptable objects as well as make attackers also able to spawn RingAttacks.
//       Remove this class once we implement a persistent attack behaviour logic, or smt similar. 
//       This attack can then be digested into a cage attack that is persistent.

// TODO: re-enable (temporarily disabled)
/// A script that will spawn a ring of attacks around given parameters. The attacks will then spin around the center point.
public class RingAttack : MonoBehaviour
{
    [SerializeField] private List<EntityType> targetTypes = new List<EntityType>();
    [SerializeField] private GameObject attackObj;
    [SerializeField] private float numAttacks = 8;
    [SerializeField] private float radius = 1f;
    public float Radius { get { return radius; } set { radius = value; } }

    private void Awake()
    {
        // Check if attackObj is not null.
        Assert.IsNotNull(attackObj, "RingAttack needs an attack object to spawn.");

        // Check if attackObj has an Attack component.
        Assert.IsNotNull(attackObj.GetComponent<AttackComponent>(), "RingAttack needs an attack object with an Attack component.");
    }

    /// <summary>
    /// Spawns the attacks around the given location.
    /// </summary>
    /// <param name="location"></param>
    public void Spawn(Transform location)
    {
        // Check if children have been spawned already.
        if (this.transform.childCount > 0)
        {
            DeSpawn();
        }

        float angleIncrement = 360f / numAttacks;

        for (int i = 0; i < numAttacks; i++)
        {
            // TODO: temp disabled.
            // Spawn the attack in a circle around the location point.
            //Vector2 offsetLoc = new Vector2(location.position.x + radius / 2, location.position.y);
            //GameObject attack = Instantiate(attackObj, offsetLoc, Quaternion.identity, this.transform);

            ////attack.GetComponent<AttackComponent>().Initialize();
            //attack.transform.RotateAround(location.position, Vector3.forward, angleIncrement * i);
        }
    }

    // Destroys all child components (all attacks).
    // TODO: refactor this so that we can reuse the attacks instead of destroying them.
    public void DeSpawn()
    {
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void Toggle(bool shouldBeActive)
    {
        if (shouldBeActive)
        {
            this.gameObject.SetActive(true);
        } else { 
            this.gameObject.SetActive(false); 
        }
    }

    private void Start()
    {
        // For testing purposes
        Spawn(this.transform);
    }

    // Bomb attack.
    //public void Spawn(Transform location)
    //{
    //    float angleIncrement = 360f / numAttacks;

    //    for (int i = 0; i < numAttacks; i++)
    //    {
    //        GameObject attack = Instantiate(attackObj, location.position, Quaternion.identity);
    //        attack.transform.RotateAround(location.position, Vector3.forward, angleIncrement * i);
    //        attack.GetComponent<Rigidbody2D>().velocity = attack.transform.up * spinSpeed;
    //    }
    //}
}
