using DG.Tweening;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CageAttacker : IAttacker
{
    public AttackerData AttackerData { get => cageAttackerData; set => cageAttackerData = (CageAttackerData) value; }
    public AttackData AttackData { get => attackData; set => attackData = value; }

    [OdinSerialize] private AttackData attackData;
    [OdinSerialize] private CageAttackerData cageAttackerData;

    private MonoBehaviour coroutineRunner; // instance that runs the coroutine.
    private Coroutine curCoroutine; // keeps track of running coroutine. If null, this means no coroutine is currently running.
    private bool attacking = false;

    public void Attack(KeyCode keyCode, AttackContext ac)
    {
        if (ac.AttackerComponent == null)
        {
            Debug.LogWarning("AttackerComponent is null. Cannot start attack coroutine.");
            return;
        }

        // If the coroutine runner has changed, update it
        if (coroutineRunner != ac.AttackerComponent)
        {
            // Stop any previous coroutines on the old runner if it exists
            if (coroutineRunner != null && curCoroutine != null)
            {
                // NOTE: this can be buggy if:
                //   1. multiple components share the same attacker. (Start and Stop conflicts..)
                coroutineRunner.StopCoroutine(curCoroutine);
            }
            coroutineRunner = ac.AttackerComponent;
        }

        if (!attacking)
        {
            attacking = true;
            curCoroutine = coroutineRunner.StartCoroutine(AttackCoroutine());
        }
    }

    private IEnumerator AttackCoroutine()
    {

        // Spawn Cage at outer radius.
        for (int i = 0; i < cageAttackerData.CageAttackDensity; i++)
        {
            // TODO: maybe we don't need coroutine since we just spawn in the attacks in a circle...
            // UPDATE: no we do b/c the attack shrink and grow at certain times.
            // Spawn in a parent object, that we can apply transform.RotateAround() on. (Refer to RingAttack.cs)
        }

        // Start Transition between growing and shrinking cage
        SetCageRadius(null, null, 1, 1);
        yield return new WaitForSeconds(0);
    }

    /// <summary>
    /// Transition to given Cage radius.
    /// </summary>
    private void SetCageRadius(List<GameObject> cageAtkProjectiles, Transform centerPoint, float radius, float transitionTime)
    {
        foreach(var projectile in cageAtkProjectiles)
        {
            // Dir from center to projectile in ring.
            Vector2 dir = projectile.transform.position - centerPoint.position;

            // Transition projectile along dir, such that the magnitude of vector from center to new location is at radius.
            Vector2 dirNormalized = dir.normalized; // Get unit vector
            Vector2 newPos = dirNormalized * radius; // Get new location.
            projectile.transform.DOMove(newPos, transitionTime); // Set new location of projectile.
        }
    }


    public void StopAttack()
    {
        attacking = false;
        throw new System.NotImplementedException();
    }

    public IAttacker DeepClone()
    {
        throw new System.NotImplementedException();
    }

    public float GetChargeUp()
    {
        return cageAttackerData.chargeUp;
    }

    public float GetCooldown()
    {
        // No Cooldown for CageAttacker
        // TODO: implement for this case in AttackComponent.
        return -1;
    }

    public bool IsInitialized()
    {
        return attackData != null && cageAttackerData != null;
    }

}
