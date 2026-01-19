using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NOTE: this class differs from other attackers, in the sense that Attack movement is controlled by this class and not an IAttackMovement class (e.g. we use NullMovement here, overrides anything set in the AttackPfb)
/// </summary>
public class CageAttacker : BaseAttacker<CageAttackerData>
{
    public override AttackData AttackData { get => attackData; set => attackData = value; }

    [Required, OdinSerialize] private AttackData attackData;

    private Sequence cageSequence;
    private float currentRadius;

    private MonoBehaviour coroutineRunner; // instance that runs the coroutine.
    private Coroutine curCoroutine; // keeps track of running coroutine. If null, this means no coroutine is currently running.
    private bool attacking = false;

    private GameObject cageRoot; // root parent that contains the multiple child component attacks of the cage.

    #region IAttack Implementation
    public override void Attack(KeyCode keyCode, AttackContext ac)
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

    #region Attack Helpers (Movement + Spawning)
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


        List<AttackComponent> projectiles = new();

        int count = Mathf.RoundToInt(attackerData.CageAttackDensity);
        float outerRadius = attackerData.CageOuterRadius;
        float innerRadius = attackerData.CageInnerRadius;

        // Spawn attacks WITHOUT movement
        var spawned = AttackSpawner.SpawnGroup(
            Mathf.RoundToInt(count),
            i =>
            {
                float angle = i * Mathf.PI * 2f / count;

                // Movement handles positioning relative to attacker
                return NullMovement.Instance;
            },
            attackData,
            ac,
            new AttackModificationContext(),
            attackData.AttackPfb.GetComponent<AttackComponent>().Attack
        );

        for (int i = 0; i < spawned.Count; i++)
        {
            var atk = spawned[i];

            // Parent under cage root
            atk.transform.SetParent(cageRoot.transform, false);

            // Initial placement on outer radius
            float angle = i * Mathf.PI * 2f / spawned.Count;
            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            atk.transform.localPosition = dir * outerRadius;

            projectiles.Add(atk);
        }

        currentRadius = outerRadius;

        // Start Transition between growing and shrinking cage
        StartCageRadiusSequence(
            projectiles,
            cageRoot.transform,
            innerRadius,
            outerRadius,
            attackerData.CycleTime
        );

       

        // Rotate Cage.
        cageRoot.transform.DORotate(
            new Vector3(0, 0, 360f),
            attackerData.RotationalSpeed,
            RotateMode.FastBeyond360
        )
        .SetEase(Ease.Linear)
        .SetLoops(-1, LoopType.Incremental);


        yield return new WaitForSeconds(0);
    }
    private void StartCageRadiusSequence(List<AttackComponent> projectiles, Transform center, float innerRadius, float outerRadius, float cycleTime)
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
    private void UpdateCageRadius(List<AttackComponent> projectiles, Transform centerPoint, float radius)
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
    #endregion

    public override void StopAttack()
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

    public override IAttacker DeepClone()
    {
        throw new System.NotImplementedException();
    }

    public override float GetChargeUp()
    {
        return attackerData.chargeUp;
    }

    public override float GetCooldown()
    {
        // No Cooldown for CageAttacker
        // TODO: implement for this case in AttackComponent.
        return -1;
    }

    public override bool IsInitialized()
    {
        return attackData != null && attackerData != null;
    }

    #endregion

    #region IAttackNode Implementation
    public override IEnumerable<IAttackNode> GetChildren()
    {
        yield return this;
    }
    #endregion
}
