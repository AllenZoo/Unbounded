using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class AttackIndicatorComponent : SerializedMonoBehaviour
{
    [OdinSerialize] private IAttackIndicator attackIndicator;

    /// <summary>
    /// Used for modifying the size of the circle.TODO; maybe move elsewhere
    /// </summary>
    //[SerializeField] private CircleScaler circleScaler;
}
