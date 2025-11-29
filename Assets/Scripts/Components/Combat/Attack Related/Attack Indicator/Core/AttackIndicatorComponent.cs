using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

/// <summary>
/// Component is attached to an entity that can trigger attack indicators.
/// NOTE: This script is NOT attached on the Indicator pfb itself! (that is handled by the AttackindicatorInstance implementation) 
/// </summary>
public class AttackIndicatorComponent : SerializedMonoBehaviour
{
    [OdinSerialize] private IAttackIndicator attackIndicator;
    [SerializeField] private Transform indicatorSpawnPoint;

    // Trigger an indicator reaction.
    public void TriggerIndicator()
    {
        var context = new AttackIndicatorContext(
            indicatorSpawnPoint.position
        );

        attackIndicator.Indicate(context);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            TriggerIndicator();
    }
}
