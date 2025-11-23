using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "FanAttackerData", menuName = "System/Combat/FanAttacker", order = 1)]
public class FanAttackerData : AttackerData
{
    public float numBlades; // Similar to numAttacks, but for continuous fan attack.
                            //public float spinSpeed;

    // TODO: functionally similar to numAttacks in AttackerData (see if we should replace)


    //public float projectileDensity; // How far apart the projectiles are spawned between each other in a single blade.
}
