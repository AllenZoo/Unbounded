using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Attack Component that is attached to Attack GameObjects
/// </summary>
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class AttackComponent : MonoBehaviour
{
    public event Action<Damageable> OnHit;
    public Attack Attack { get { return attack; } private set { } }
    public List<EntityType> TargetTypes {  get { return targetTypes; } set { targetTypes = value ?? new List<EntityType>(); } }

    [Tooltip("The projectile data associated with Attack")]
    [Required, SerializeField] private Attack attack;

    private List<EntityType> targetTypes = new List<EntityType>();
    private List<Damageable> hitTargets = new List<Damageable>();

    private void Awake()
    {
        // Checks to see RB2 and Collider2D components properties are correct.

        // Check if RB2 is kinematic.
        Assert.IsTrue(GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Kinematic, "RB2D needs to be kinematic");

        // Check if Collider2D is a trigger.
        Assert.IsTrue(GetComponent<Collider2D>().isTrigger, "Collider2D needs to be a trigger");

        // Checks if layer is on 'AttackCollider'
        // Used to optimize collision detections
        // TODO:
        //Assert.IsTrue(gameObject.layer.Equals("AttackCollider"));
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

   
    /// <summary>
    /// Helper that validates and then triggers the hit.
    /// </summary>
    /// <param name="hit">the target we hit</param>
    /// <returns>whether the hit is valid</returns>
    private bool ValidateHit(Damageable hit)
    {
        // Check if hit target EntityType matches what the attack can hit.
        if (!targetTypes.Contains(hit.EntityType))
        {
            // Attack can't hit this target.
            return false;
        }

        // Checks if hit already has been processed by this attack on this target.
        if (!attack.AttackData.canRepeat && hitTargets.Contains(hit))
        {
            // Attack doesn't repeat damage, and already has hit this target.
            return false;
        }

        // Trigger Hit
        attack.Hit(hit, this.transform);
        hitTargets.Add(hit);


        // Resets the attack if conditions are met.
        if (!attack.AttackData.isAOE && !attack.AttackData.isPiercing && !attack.AttackData.lastsUntilDuration)
        {
            // Destroy the attack object. (or set inactive if we want to reuse it)
            ResetAttack();
        }

        return true;
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

}
