using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script attached to attack objects that contain info about the attack.
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Attack : MonoBehaviour
{
    public event Action<Damageable> OnHit;

    // TODO: Split these fields into different component classes. eg. DOT component, AOE component, etc.
    [SerializeField] private float damage = 5f;

    [Tooltip("If true, the attack will hit all targets in the collider. If false, it will only hit the first target.")]
    [SerializeField] private Boolean isAOE = false;

    [Tooltip("If true, the attack will be able to hit the same target multiple times.")]
    [SerializeField] private Boolean canRepeat = false;

    [Tooltip("If true, the attack will deal damage over time.")]
    [SerializeField] private Boolean isDOT = false;
    [SerializeField] private float dotDuration = 5f;

    private List<Damageable> hitTargets = new List<Damageable>();

    private void Start()
    {
        OnHit += Hit;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable target = collision.GetComponent<Damageable>();
        if (target != null)
        {
            OnHit?.Invoke(target);
        }
    }

    // Logic to determine what happens when the attack hits a target.
    private void Hit(Damageable hit)
    {
        // Checks if hit already has been processed by this attack on this target.
        if (!canRepeat && hitTargets.Contains(hit))
        {
            // Attack doesn't repeat damage, and already has hit this target.
            return;
        }

        if (!isAOE)
        {
            // Destroy the attack object. (or set inactive if we want to reuse it)
            gameObject.SetActive(false);
        }

        if (isDOT)
        {
            hit.TakeDamageOverTime(this);
            return; 
        }

        hit.TakeDamage(damage);
        hitTargets.Add(hit);
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
    #endregion
}
