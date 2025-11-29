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


    private void Update()
    {
        // For testing purposes only
        if (Input.GetKeyDown(KeyCode.T))
        {
            attackIndicator.Test(this.gameObject);
        }

    }
}
