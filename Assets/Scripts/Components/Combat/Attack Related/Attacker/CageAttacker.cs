using Sirenix.Serialization;
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

            // Spawn Cage at outer radius.
            for (int i = 0; i < cageAttackerData.CageAttackDensity; i++)
            {

            }

            // Start Transition between growing and shrinking cage

        }

        throw new System.NotImplementedException();
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
