using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// Intended to be a callback from the Damageable component's TakeDamage event and spawn a damage number for user clarity.
public class DamageNumberSpawner : MonoBehaviour
{
    [Tooltip("The prefab to spawn when damage is taken.")]
    [SerializeField] private GameObject damageNumberPfb;
    [SerializeField] private Damageable damageableRef;

    private void Awake()
    {
        if (damageableRef == null)
        {
            damageableRef = GetComponent<Damageable>();
        }
        Assert.IsNotNull(damageableRef, "Damage numbers needs a damageable ref so it knows when to spawn damage numbers.");

        // Check that damageNumberPfb has a DamageNumber component
        Assert.IsNotNull(damageNumberPfb.GetComponent<DamageNumber>(), "DamageNumberSpawner needs a prefab with a DamageNumber component.");
    }

    private void Start()
    {
        damageableRef.OnDamage += SpawnDamageNumber;
    }

    private void SpawnDamageNumber(float damageTaken)
    {
        GameObject damageNumber = Instantiate(damageNumberPfb, transform.position, Quaternion.identity, transform);
        damageNumber.GetComponent<DamageNumber>().SetDamageNumber(damageTaken);
        damageNumber.GetComponent<DamageNumber>().StartDestroyTimer();
    }
}
