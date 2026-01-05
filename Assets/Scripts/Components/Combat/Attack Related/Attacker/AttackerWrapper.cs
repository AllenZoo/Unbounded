using UnityEngine;

/// <summary>
/// Class that wraps multiple different kinds of IAttackers so that they can all 'attack' at once.
/// 
/// Note: CD is overwritten by this class.
/// </summary>
public class AttackerWrapper : IAttacker
{
    public AttackerData AttackerData { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public AttackData AttackData { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public void Attack(KeyCode keyCode, AttackContext attackContext)
    {
        throw new System.NotImplementedException();
    }

    public bool CanAttack()
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
