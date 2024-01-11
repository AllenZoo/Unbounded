using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// Script attached to attack objects that contain info about the attack.
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Attack : MonoBehaviour
{
    public event Action<Damageable> OnHit;
    [SerializeField] public List<EntityType> TargetTypes = new List<EntityType>();

    // TODO: Split these fields into different component classes. eg. DOT component, AOE component, etc.
    [SerializeField] private string attackName = "Attack";
    [SerializeField] private float damage = 5f;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float initialSpeed = 0f;

    [Tooltip("time it takes to charge up attack.")]
    [SerializeField] private float chargeUp = 0f;
    [SerializeField] private float knockback = 0f;
    [SerializeField] private float stunDuration = 1f;

    [Tooltip("If true, the attack will hit all targets in the collider. If false, it will only hit the first target.")]
    [SerializeField] private Boolean isAOE = false;

    [Tooltip("If true, the attack will be able to hit the same target multiple times.")]
    [SerializeField] private Boolean canRepeat = false;

    [Tooltip("If true, the attack will deal damage over time.")]
    [SerializeField] private Boolean isDOT = false;
    [SerializeField] private float dotDuration = 5f;

    [Tooltip("If true, the attack will pierce through targets.")]
    [SerializeField] private Boolean isPiercing = false;

    [Tooltip("If true, the attack will last until duration is over.")]
    [SerializeField] private Boolean lastsUntilDuration = false;

    [SerializeField] private List<Damageable> hitTargets = new List<Damageable>();

    [Header("For rendering")]
    [SerializeField] private float rotOffset;

    private void Awake()
    {
        // Checks to see RB2 and Collider2D components properties are correct.
        
        // Check if RB2 is kinematic.
        Assert.IsTrue(GetComponent<Rigidbody2D>().isKinematic, "RB2D needs to be kinematic");

        // Check if Collider2D is a trigger.
        Assert.IsTrue(GetComponent<Collider2D>().isTrigger, "Collider2D needs to be a trigger");
    }

    private void Start()
    {
        OnHit += Hit;
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
        if (!canRepeat && hitTargets.Contains(hit))
        {
            // Attack doesn't repeat damage, and already has hit this target.
            return;
        }

        // Damage the target.
        if (isDOT)
        {
            hit.TakeDamageOverTime(this);
            return;
        }

        hit.TakeDamage(damage);
        hitTargets.Add(hit);
        
        // Knockback the target if:
        //      - attack has knockback
        //      - target is knockbackable
        if (knockback > 0)
        {
            Knockbackable kb = hit.GetComponent<Knockbackable>();
            if (kb != null)
            {
                kb.Knockback(hit.transform.position - this.transform.position, knockback, stunDuration);
            }
        }


        // Resets the attack if conditions are met.
        if (!isAOE && !isPiercing && !lastsUntilDuration)
        {
            // Destroy the attack object. (or set inactive if we want to reuse it)
            ResetAttack();
            return;
        }
    }

    #region Getters and Setters
    public float Damage
    {
        get { return damage; }
    }

    public float DotDuration
    {
        get { return dotDuration; }
    }

    public float Duration
    {
        get { return duration; }
    }

    public string Name
    {
        get { return attackName; }
    }

    public float InitialSpeed
    {
        get { return initialSpeed; }
    }

    public float ChargeUp
    {
        get { return chargeUp; }
    }

    public float RotOffset
    {
        get { return rotOffset; }
    }
    #endregion
}
