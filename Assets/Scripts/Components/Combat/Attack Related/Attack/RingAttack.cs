using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// TODO-OPT: refactor this an attack script to use scriptable objects as well as make attackers also able to spawn RingAttacks.
//       Remove this class once we implement a persistent attack behaviour logic, or smt similar. 
//       This attack can then be digested into a cage attack that is persistent.
// Note: temporarily, this implementation is good, don't change unless necessary.
/// A script that will spawn a ring of attacks around given parameters. The attacks will then spin around the center point.
public class RingAttack : MonoBehaviour
{
    [Required, SerializeField] private AttackerComponent attackerComp;
    [SerializeField] private GameObject attackObj;
    [SerializeField] private float numAttacks = 8;
    [SerializeField] private float radius = 1f;

    public float Radius { get { return radius; } set { radius = value; } }

    private List<AttackComponent> spawnedAttacks = new List<AttackComponent>();

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

        AttackData attackData = attackObj.GetComponent<AttackComponent>().Attack.AttackData;
        AttackContext ac = new AttackContext
        {
            AttackerComponent = attackerComp,
        };

        // Spawn attacks WITHOUT movement
        var spawned = AttackSpawner.SpawnGroup(
            Mathf.RoundToInt(numAttacks),
            i =>
            {
                float angle = i * Mathf.PI * 2f / numAttacks;

                // Movement handles positioning relative to attacker
                return NullMovement.Instance;
            },
            attackData,
            ac,
            new AttackModificationContext(),
            attackData.AttackPfb.GetComponent<AttackComponent>().Attack,
            rotationFactory: i =>
                Quaternion.Euler(0f, 0f, i * (360f / numAttacks))
        );

        spawnedAttacks = spawned;

        for (int i = 0; i < spawned.Count; i++)
        {
            var atk = spawned[i];

            // Parent under cage root
            atk.transform.SetParent(this.transform, false);

            // Initial placement on outer radius
            float angle = i * Mathf.PI * 2f / spawned.Count;
            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            atk.transform.localPosition = dir * radius;

        }
    }

    // Destroys all child components (all attacks).
    // TODO: refactor this so that we can reuse the attacks instead of destroying them.
    public void DeSpawn()
    {
        foreach (var attack in spawnedAttacks)
        {
            Destroy(attack.gameObject);
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
}
