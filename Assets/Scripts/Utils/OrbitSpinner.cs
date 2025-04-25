using UnityEngine;
using UnityEngine.Assertions;

// Spins a given object around a point
public class OrbitSpinner : MonoBehaviour
{
    [Tooltip("Object to spin around a point.")]
    [SerializeField] private GameObject obj;
    [SerializeField] private float speed = 1f;
    // [SerializeField] private float radius = 1f;
    [Tooltip("Center point to spin around.")]
    [SerializeField] private Transform center;
    [SerializeField] private bool spinClockWise = true;

    private static float SPIN_SPEED_SCALE = 20f;

    private void Awake()
    {
        Assert.IsNotNull(obj, "Orbit spinner needs an object to spin.");
        Assert.IsNotNull(center, "Orbit spinner needs a center point to spin around.");
    }

    private void FixedUpdate()
    {
        // Debug.Log("Center position: " + center.position);
        if (obj != null)
        {
            // Spin the object around the center point.
            if (spinClockWise)
            {
                obj.transform.RotateAround(center.position, Vector3.back, speed * SPIN_SPEED_SCALE * Time.deltaTime);
            }
            else
            {
                obj.transform.RotateAround(center.position, Vector3.forward, speed * SPIN_SPEED_SCALE * Time.deltaTime);
            }

        }
    }

}
