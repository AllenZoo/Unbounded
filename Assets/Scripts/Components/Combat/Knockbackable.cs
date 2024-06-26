using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Knockbackable : MonoBehaviour
{
    [SerializeField] private LocalEventHandler localEventHandler;

    [SerializeField] private Rigidbody2D rb;

    [Tooltip("Here to be able to toggle, as there are some enemies that we want to be knockbackable only during certain conditions." +
        "Eg. bosses and phases.")]
    [SerializeField] private bool currentlyKnockbackable = true;

    private void Awake()
    {
        Assert.IsNotNull(rb, "For something to be knockbackable, it needs a Rigidbody2d!");

        // Check if Rb2 is dynamic
        Assert.IsTrue(rb.bodyType == RigidbodyType2D.Dynamic, "Rb2D needs to be dynamic");

        // Check if mass of rb2 is 5
        Assert.IsTrue(rb.mass == 5, "Rb2D needs to have a mass of 5");

        // Set mass of rb2 to 5
        rb.mass = 5;

        if (localEventHandler == null)
        {
            localEventHandler = GetComponentInParent<LocalEventHandler>();
            if (localEventHandler == null)
            {
                Debug.LogError("LocalEventHandler unassigned and not found in parent for object [" + gameObject +
                                       "] with root object [" + gameObject.transform.root.name + "] for Knockbackable.cs");
            }
        }
    }

    // Knockbacks the attached entity if currentlyKnockbackable is true.
    public void Knockback(Vector2 direction, float force, float duration)
    {
        if (!currentlyKnockbackable)
        {
            Debug.Log("Tried to knockback currently unknockbackable entity.");
            return;
        }
        StartCoroutine(ApplyKnockback(direction, force, duration));
    }

    public void ToggleKnockbackability(bool knockbackable)
    {
        currentlyKnockbackable = knockbackable;
    }

    private IEnumerator ApplyKnockback(Vector2 direction, float force, float duration)
    {
        // Debug.Log("Knockback start!");
        localEventHandler.Call(new OnKnockBackBeginEvent { knockbackDir = direction, knockbackForce = force });
        rb.velocity = Vector2.zero;
        rb.AddForce(direction * force, ForceMode2D.Impulse);
        yield return new WaitForSeconds(duration);
        rb.velocity = Vector2.zero;
        // Debug.Log("Knockback ended!");
        localEventHandler.Call(new OnKnockBackEndEvent());
    }
}
