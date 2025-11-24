using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "FanAttackerData", menuName = "System/Combat/FanAttacker", order = 1)]
public class FanAttackerData : AttackerData
{
    // TODO: functionally similar to numAttacks in AttackerData (see if we should replace)
    public float numBlades; // Similar to numAttacks, but for continuous fan attack.

    [Tooltip("The speed of which the blade spins in degrees per second. (deg/s)")]
    public float spinSpeed; // How fast the fan spins (degrees per second).

    [Tooltip("The time between each projectile of a single blade being spawned. [Projectile Density] (s)")]
    public float timeBetweenBladeProjectiles; // How far apart the projectiles are spawned between each other in a single blade.

    public bool clockwiseSpin = true; // Whether the fan spins clockwise or counter-clockwise.
}
