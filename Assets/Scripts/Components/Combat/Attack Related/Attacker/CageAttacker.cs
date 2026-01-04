using DG.Tweening;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static System.TimeZoneInfo;

public class CageAttacker : IAttacker
{
    public AttackerData AttackerData { get => cageAttackerData; set => cageAttackerData = (CageAttackerData) value; }
    public AttackData AttackData { get => attackData; set => attackData = value; }

    [OdinSerialize] private AttackData attackData;
    [OdinSerialize] private CageAttackerData cageAttackerData;

    private DG.Tweening.Sequence cageSequence;
    private float currentRadius;


    private MonoBehaviour coroutineRunner; // instance that runs the coroutine.
    private Coroutine curCoroutine; // keeps track of running coroutine. If null, this means no coroutine is currently running.
    private bool attacking = false;

    private GameObject cageRoot; // root parent that contains the multiple child component attacks of the cage.

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
            curCoroutine = coroutineRunner.StartCoroutine(AttackCoroutine(ac));
        }
    }

    private IEnumerator AttackCoroutine(AttackContext ac)
    {
        // Redundant Cleanup.
        if (cageRoot != null)
        {
            cageRoot.transform.DOKill();
            GameObject.Destroy(cageRoot);
        }


        // Create CageRootParent on Attacker.
        cageRoot = new GameObject("CageRoot");
        cageRoot.transform.SetParent(ac.AttackerTransform, false);
        cageRoot.transform.localPosition = Vector3.zero;


        List<GameObject> projectiles = new ();

        float count = cageAttackerData.CageAttackDensity;
        float outerRadius = cageAttackerData.CageOuterRadius;
        float innerRadius = cageAttackerData.CageInnerRadius;

        // Spawn Cage at outer radius.
        for (int i = 0; i < count; i++)
        {
            // TODO: maybe we don't need coroutine since we just spawn in the attacks in a circle...
            // UPDATE: no we do b/c the attack shrink and grow at certain times.
            // Spawn in a parent object, that we can apply transform.RotateAround() on. (Refer to RingAttack.cs)

            float angle = i * Mathf.PI * 2f / count;
            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            Vector2 spawnPos = (Vector2) cageRoot.transform.position + dir * outerRadius;

            // Instantiate projectile.
            GameObject proj = Object.Instantiate(
                   attackData.attackPfb,
                   spawnPos,
                   Quaternion.identity,
                   cageRoot.transform
            );

            projectiles.Add(proj);
        }

        currentRadius = outerRadius;

        // Start Transition between growing and shrinking cage
        StartCageRadiusSequence(
            projectiles,
            cageRoot.transform,
            innerRadius,
            outerRadius,
            cageAttackerData.CycleTime
        );

       

        // Rotate Cage.
        cageRoot.transform.DORotate(
            new Vector3(0, 0, 360f),
            cageAttackerData.RotationalSpeed,
            RotateMode.FastBeyond360
        )
        .SetEase(Ease.Linear)
        .SetLoops(-1, LoopType.Incremental);


        yield return new WaitForSeconds(0);
    }


    private void StartCageRadiusSequence(List<GameObject> projectiles, Transform center, float innerRadius, float outerRadius, float cycleTime)
    {
        cageSequence?.Kill();

        cageSequence = DOTween.Sequence();

        float half = cycleTime * 0.5f;

        // Outer -> Inner
        cageSequence.Append(
            DOTween.To(
                () => currentRadius,
                r =>
                {
                    currentRadius = r;
                    UpdateCageRadius(projectiles, center, r);
                },
                innerRadius,
                half
            ).SetEase(Ease.InOutSine)
        );

        // Inner -> Outer
        cageSequence.Append(
            DOTween.To(
                () => currentRadius,
                r =>
                {
                    currentRadius = r;
                    UpdateCageRadius(projectiles, center, r);
                },
                outerRadius,
                half
            ).SetEase(Ease.InOutSine)
        );

        cageSequence.SetLoops(-1);
    }

    private void UpdateCageRadius(List<GameObject> projectiles, Transform centerPoint, float radius)
    {
        foreach (var projectile in projectiles)
        {
            // Dir from center to projectile in ring.
            Vector2 dir = projectile.transform.position - centerPoint.position;

            // Transition projectile along dir, such that the magnitude of vector from center to new location is at radius.
            Vector2 dirNormalized = dir.normalized; // Get unit vector
            Vector2 newPos = (Vector2)centerPoint.position + dirNormalized * radius; // Get new location.

            projectile.transform.position = newPos;
        }
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
            Vector2 newPos = (Vector2)centerPoint.position + dirNormalized * radius; // Get new location.
            projectile.transform.DOMove(newPos, transitionTime).SetEase(Ease.InOutSine); // Set new location of projectile.
        }
    }


    public void StopAttack()
    {
        if (cageRoot != null)
        {
            // Destroy projectiles
            cageRoot.transform.DOKill();
            GameObject.Destroy(cageRoot);
            cageRoot = null;
        }
       
        attacking = false;
    }

    public bool CanAttack()
    {
        // We can attack if we aren't already attacking.
        return !attacking;
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
