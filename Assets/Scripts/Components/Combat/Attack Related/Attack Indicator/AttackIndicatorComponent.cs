using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class AttackIndicatorComponent : SerializedMonoBehaviour
{
    [OdinSerialize] private IAttackIndicator attackIndicator;
}
