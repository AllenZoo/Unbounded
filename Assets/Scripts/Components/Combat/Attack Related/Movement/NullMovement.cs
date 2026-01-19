using UnityEngine;

/// <summary>
/// Note: Sealed b/c we don't want any class to inherit this.
/// </summary>
public sealed class NullMovement : IAttackMovement
{
    public static readonly NullMovement Instance = new NullMovement();

    private NullMovement() { }

    public void Init(AttackComponent ac, AttackData data, AttackContext context, AttackModificationContext amc)
    {

    }

    public void UpdateMovement(AttackComponent ac, Rigidbody2D rb)
    {

    }


}
