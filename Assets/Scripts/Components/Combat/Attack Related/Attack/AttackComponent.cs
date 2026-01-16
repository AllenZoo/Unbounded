using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Attack Component that is attached to Attack GameObjects
/// </summary>
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class AttackComponent : SerializedMonoBehaviour
{
    public Rigidbody2D Rb { get; private set; }
    public Collider2D AttackCollider { get; private set; }
    //public AttackData Data { get; private set; } // Note: doesn't seem like this is even used..


    public IAttack Attack { get { return attack; } private set { } }
    public IAttackMovement Movement { get { return movement; } private set { } }


    [Tooltip("The projectile data associated with Attack")]
    [Required, OdinSerialize] private IAttack attack;

    [Tooltip("The movement data associated with Attack")]
    [Required, OdinSerialize] private IAttackMovement movement;

    private AttackContext attackContext;


    //public List<EntityType> TargetTypes {  get { return targetTypes; } set { targetTypes = value ?? new List<EntityType>(); } }
    //[Header("Debugging (Reset fields to empty)")]
    //[SerializeField] private List<EntityType> targetTypes = new List<EntityType>();

    [SerializeField] private List<Damageable> hitTargets = new List<Damageable>();
    public event Action<Damageable> OnHit;

    private void Awake()
    {
        // Checks to see RB2 and Collider2D components properties are correct.

        // Check if RB2 is kinematic.
        Assert.IsTrue(GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Kinematic, "RB2D needs to be kinematic");

        // Check if Collider2D is a trigger.
        Assert.IsTrue(GetComponent<Collider2D>().isTrigger, "Collider2D needs to be a trigger");

        AttackCollider = GetComponent<Collider2D>();
        Rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(AttackData data, AttackContext context, IAttack logic, IAttackMovement movement)
    {
        //Data = data;
        Movement = movement;
        Attack = logic;
        this.attackContext = context;
        movement.UpdateMovement(this, Rb);
    }

    /// <summary>
    /// For detecting hits against Damageable components!
    /// </summary>
    /// <param name="collision"></param>
    public void OnTriggerEnter2D(Collider2D collision)
    {

        // All Damageable objects have a collider2d and Damageable component.
        Damageable target = collision.GetComponent<Damageable>();
        if (target != null && target.isHittable)
        {
            if (ValidateHit(target))
            {
                OnHit?.Invoke(target);
            }
            
        }
    }

    public void TriggerAttackLaunch()
    {
        attack.OnLaunch(this);
    }

    public void TriggerAttackLand()
    {
        // Get all hittable entities in circle at time.
        attack.OnLand(this);
    }

    // For resetting the attack when it is disabled.
    //   - Clears hitTargets list.
    //   - Sets attack object to inactive.
    public void ResetAttack()
    {
        StopAllCoroutines();
        hitTargets.Clear();
        attack.Reset(this);

        if (attack.AttackData.DisappearOnHit)
        {
            this.gameObject.SetActive(false);
        } else
        {
            // object still needs to disable itself 
            // TODO: Remove the following line and implement new logic.
            // Maybe think of new way to implement this difference between bomb attack and projectile attack? (AOE DOT vs  projectile)
            this.gameObject.SetActive(false);


            // UPDATE: why not just use duration?
        }
        
    }

    public void ResetAttackAfterTime(float time)
    {
        StartCoroutine(ResetAttackAfterTimeCoroutine(time));
    }

    /// <summary>
    /// Helper that validates and then triggers the hit.
    /// </summary>
    /// <param name="hit">the target we hit</param>
    /// <returns>whether the hit is valid</returns>
    private bool ValidateHit(Damageable hit)
    {
        AttackData ad = attack.AttackData;

        if (attackContext == null)
        {
            Debug.LogError("AttackContext is NULL", this);
        }
        else if (attackContext.AttackerComponent == null)
        {
            Debug.LogError("AttackContext.AttackerComponent is NULL", this);
        }
        else if (attackContext.AttackerComponent.TargetTypes == null)
        {
            Debug.LogError("AttackContext.AttackerComponent.TargetTypes is NULL", this);
        }
        else if (!attackContext.AttackerComponent.TargetTypes.Contains(hit.EntityType))
        {
            //Debug.Log($"Hit rejected: EntityType {hit.EntityType} not in TargetTypes", this);
        }


        // Check if hit target EntityType matches what the attack can hit.
        if (!attackContext.AttackerComponent.TargetTypes.Contains(hit.EntityType))
        {
            // Attack can't hit this target.
            return false;
        }

        // Checks if hit already has been processed by this attack on this target.
        if (hitTargets.Contains(hit))
        {
            // Attack already has hit this target.
            return false;
        }

        // Trigger Hit
        var hitSuccess = attack.Hit(hit, this.transform);
        if (hitSuccess) hitTargets.Add(hit);

        // Reset hit cooldown if attack does not disappear after a hit, and if the attack can repeat hit a target.
        if (!ad.DisappearOnHit && ad.CanRepeat)
        {
            if (ad.RehitCooldown <= 0)
            {
                hitTargets.Remove(hit);
            } else
            {
                StartCoroutine(ResetHitCooldown(ad.RehitCooldown, hit));
            }
        }


        // Resets the attack if conditions are met.
        if (!ad.IsAOE && !ad.IsPiercing && ad.DisappearOnHit)
        {
            // Destroy the attack object. (or set inactive if we want to reuse it)
            ResetAttack();
        }

        return true;
    }

    private IEnumerator ResetHitCooldown(float delay, Damageable target)
    {
        yield return new WaitForSeconds(delay);
        if (hitTargets.Contains(target))
        {
            hitTargets.Remove(target);
        }
    }

    private IEnumerator ResetAttackAfterTimeCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        ResetAttack();
    }


}
