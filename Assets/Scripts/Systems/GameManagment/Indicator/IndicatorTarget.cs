using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Component attached to any gameobjects that want to spawn in an indicator pointing torwards it.
/// </summary>
public class IndicatorTarget : MonoBehaviour
{
    [Required, SerializeField] private Transform targetTransform;
    [SerializeField] private bool createIndicatorOnStart = true;

    private void Start()
    {
        if (IndicatorSystem.Instance == null) Debug.LogError("Indicator System Reference is null!");

        if (createIndicatorOnStart)
        {
            Debug.Log("Creating Indicator!");
            IndicatorSystem.Instance.CreateIndicator(targetTransform);
        }
    }

    //private void Update()
    //{
    //    // TODO: remove after debugging
    //    if (Input.GetKeyDown(KeyCode.P))
    //    {
    //        Debug.Log("Being called");
    //        IndicatorSystem.Instance.CreateIndicator(targetTransform);
    //    }
    //}
}
