using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script attached to attack objects that contain info about the attack.
[RequireComponent(typeof(Collider2D))]
public class Attack : MonoBehaviour
{
    public event Action<Damageable> OnHit;

    // TODO: move these data fields to a scriptable object.
    [SerializeField] private float damage = 5f;
    [SerializeField] private Boolean isAOE = false;

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
        if (!isAOE)
        {
            // Destroy the attack object. (or set inactive if we want to reuse it)
            gameObject.SetActive(false);
        }
        hit.TakeDamage(damage);
    }
}
