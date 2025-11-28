using Sirenix.Serialization;
using UnityEngine;

public class MeteorAttacker : IAttacker
{
    public AttackerData AttackerData { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public AttackData AttackData { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }


    // TODO: create an indicator class (creates indicator that transitions from transparent to opaque over time)
    [OdinSerialize]
    private IAttackIndicator attackIndicator;

    public void Attack(KeyCode keyCode, AttackContext attackContext)
    {

        // TODO: randomly generate meteor positions within a certain area around the target position
        throw new System.NotImplementedException();

        // TODO: for each meteor position, spawn an indicator and after a delay, spawn the meteor attack
    }

    public IAttacker DeepClone()
    {
        throw new System.NotImplementedException();
    }

    public float GetChargeUp()
    {
        throw new System.NotImplementedException();
    }

    public float GetCooldown()
    {
        throw new System.NotImplementedException();
    }

    public bool IsInitialized()
    {
        throw new System.NotImplementedException();
    }

    public void StopAttack()
    {
        throw new System.NotImplementedException();
    }
}
