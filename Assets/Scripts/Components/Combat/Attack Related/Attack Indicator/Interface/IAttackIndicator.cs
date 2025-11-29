using System.Collections.Generic;
using UnityEngine;

public interface IAttackIndicator
{
    /// <summary>
    /// Spawns indicator at the appropriate position with the appropriate parameters.
    /// </summary>
    /// <param name="context"></param>
    void Indicate(AttackIndicatorContext context);
}
