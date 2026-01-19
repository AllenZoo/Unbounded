using UnityEngine;

public interface IAttackMovement
{
 
    /// <summary>
    /// Function to prep the Attack for movement
    /// </summary>
    /// <param name="ac"></param>
    /// <param name="data"></param>
    /// <param name="context"></param>
    void Init(AttackComponent ac, AttackData data, AttackContext context, AttackModificationContext amc);

    /// <summary>
    /// Function that starts the movement of Attack.
    /// </summary>
    /// <param name="ac"></param>
    /// <param name="rb"></param>
    void UpdateMovement(AttackComponent ac, Rigidbody2D rb);
}
