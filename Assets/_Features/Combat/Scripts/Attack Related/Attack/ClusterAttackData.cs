using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Cluster Attack Data", menuName = "System/Combat/Attack/ClusterAttack")]
public class ClusterAttackData : AttackData
{
    [Header("Cluster Settings")]
    [Tooltip("The attack data used for the child projectiles.")]
    [Required] public AttackData SubAttackData;

    [Tooltip("How many child projectiles to spawn.")]
    public int ClusterCount = 3;

    [Tooltip("The spread angle of the cluster.")]
    public float ClusterSpreadAngle = 45f;

    [Tooltip("If true, children spawn in a full circle. If false, they use the spread angle relative to current velocity.")]
    public bool UseFullCircle = false;

    [Tooltip("Delay before splitting. If 0, it might split on Hit or Land instead.")]
    public float SplitDelay = 0.5f;
}