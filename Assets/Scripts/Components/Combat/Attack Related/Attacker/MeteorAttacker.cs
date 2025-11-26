using UnityEngine;

public class MeteorAttacker : IAttacker
{
    public AttackerData AttackerData { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public AttackData AttackData { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }


    // TODO: create an indicator class (creates indicator that transitions from transparent to opaque over time)
    

    public void Attack(KeyCode keyCode, AttackContext attackContext)
    {
        throw new System.NotImplementedException();
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
