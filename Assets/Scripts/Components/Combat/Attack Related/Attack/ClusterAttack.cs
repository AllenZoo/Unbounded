using System.Collections;
using UnityEngine;

public class ClusterAttack : BaseAttack<ClusterAttackData>
{

    public override void OnLand(AttackComponent ac)
    {
        // Do nothing for Cluster Attack.
        //Split(ac);
    }

    public override void OnLaunch(AttackComponent ac)
    {
        // If there is a timer-based split, start it here
        if (attackData.SplitDelay > 0)
        {
            ac.StartCoroutine(SplitTimer(ac));
        }
    }

    private IEnumerator SplitTimer(AttackComponent ac)
    {
        yield return new WaitForSeconds(attackData.SplitDelay);
        Split(ac);

        // After splitting mid-air, usually the parent disappears
        ac.ResetAttack();
    }

    private void Split(AttackComponent ac)
    {
        if (ac == null) return;

        // We need the original context to ensure the sub-attacks know who the attacker is
        // NOTE: Ensure ac.attackContext is made public or internal in AttackComponent.cs
        AttackContext originalContext = ac.AttackContext;

        float baseAngle = Mathf.Atan2(ac.Rb.linearVelocity.y, ac.Rb.linearVelocity.x) * Mathf.Rad2Deg;

        for (int i = 0; i < attackData.ClusterCount; i++)
        {
            float angleOffset;
            if (attackData.UseFullCircle)
            {
                angleOffset = i * (360f / attackData.ClusterCount);
            }
            else
            {
                // Calculate fan spread
                float startAngle = -attackData.ClusterSpreadAngle / 2f;
                float step = attackData.ClusterCount > 1 ? attackData.ClusterSpreadAngle / (attackData.ClusterCount - 1) : 0;
                angleOffset = startAngle + (step * i);
            }

            Vector3 spawnDir = Quaternion.Euler(0, 0, baseAngle + angleOffset) * Vector3.right;

            // Prepare Modification Context for the child
            AttackModificationContext amc = new AttackModificationContext()
            {
                AttackDirection = spawnDir,
                AttackDuration = attackData.SubAttackData.Duration,
                ObjectScale = 1f // Or inherit from parent
            };

            // Get components from the sub-attack prefab
            var subAtkComp = attackData.SubAttackData.AttackPfb.GetComponent<AttackComponent>();

            // Spawn the sub-attack
            AttackSpawner.Spawn(
                attackData.SubAttackData,
                originalContext,
                amc,
                subAtkComp.Attack,
                subAtkComp.Movement
            );
        }
    }

    public override void Reset(AttackComponent ac) { }

}
