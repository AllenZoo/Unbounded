using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// Script attached to attack objects that contain info about the attack.
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Attack : MonoBehaviour
{
    public event Action<Damageable> OnHit;
    [SerializeField] public List<EntityType> TargetTypes = new List<EntityType>();

    [SerializeField] private List<Damageable> hitTargets = new List<Damageable>();

    [SerializeField] private SO_Attack data;

    public AttackData Data
    {
        get { return data.data; }
        set { data.data = value; }
    }

    public float attackerATKStat { private get; set; } = 0;

    private void Awake()
    {
        // Checks to see RB2 and Collider2D components properties are correct.
        
        // Check if RB2 is kinematic.
        Assert.IsTrue(GetComponent<Rigidbody2D>().isKinematic, "RB2D needs to be kinematic");

        // Check if Collider2D is a trigger.
        Assert.IsTrue(GetComponent<Collider2D>().isTrigger, "Collider2D needs to be a trigger");

        // Check that data is not null.
        Assert.IsNotNull(data, "Attack needs data to function");
    }

    private void Start()
    {
        OnHit += Hit;

        if (data != null)
        {
            // Init Avoid pass by ref.
            Data = data.data.Copy();   
        }
    }


    public void OnTriggerEnter2D(Collider2D collision)
    {

        // All Damageable objects have a collider2d and Damageable component.
        Damageable target = collision.GetComponent<Damageable>();
        if (target != null && target.isHittable)
        {
            OnHit?.Invoke(target);
        }
    }

    // For resetting the attack when it is disabled.
    //   - Clears hitTargets list.
    //   - Sets attack object to inactive.
    public void ResetAttack()
    {
        StopAllCoroutines();
        hitTargets.Clear();
        this.gameObject.SetActive(false);
    }

    public void ResetAttackAfterTime(float time)
    {
        StartCoroutine(ResetAttackAfterTimeCoroutine(time));
    }

    private IEnumerator ResetAttackAfterTimeCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        ResetAttack();
    }

    // Logic to determine what happens when the attack hits a target.
    private void Hit(Damageable hit)
    {
        // Check if hit target EntityType matches what the attack can hit.
        if (!TargetTypes.Contains(hit.EntityType))
        {
            // Attack can't hit this target.
            return;
        }

        // Checks if hit already has been processed by this attack on this target.
        if (!Data.canRepeat && hitTargets.Contains(hit))
        {
            // Attack doesn't repeat damage, and already has hit this target.
            return;
        }

        float calculatedDamage = CalculateDamage(Data.damage, attackerATKStat);

        // Damage the target.
        if (Data.isDOT)
        {
            hit.TakeDamageOverTime(this, calculatedDamage);
            return;
        }

        hit.TakeDamage(calculatedDamage);
        hitTargets.Add(hit);
        
        // Knockback the target if:
        //      - attack has knockback
        //      - target is knockbackable
        if (Data.knockback > 0)
        {
            Knockbackable kb = hit.GetComponent<Knockbackable>();
            if (kb != null)
            {
                kb.Knockback(hit.transform.position - this.transform.position, Data.knockback, Data.stunDuration);
            }
        }


        // Resets the attack if conditions are met.
        if (!Data.isAOE && !Data.isPiercing && !Data.lastsUntilDuration)
        {
            // Destroy the attack object. (or set inactive if we want to reuse it)
            ResetAttack();
            return;
        }
    }

    // Calculates the damage of the attack while also taking into account the attacker's stats.
    // TODO: move this logic to a util class alongside with the damage calculation formula in Damageable.
    private float CalculateDamage(float baseDamage, float atkStat)
    {
        float calculatedDMG = baseDamage + atkStat;
        return calculatedDMG;

    }
}
