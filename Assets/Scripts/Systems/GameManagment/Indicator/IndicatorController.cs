using UnityEngine;

public class IndicatorController : MonoBehaviour
{
    [SerializeField] private IndicatorView view;

    [SerializeField] private Transform objToPointTo;
    [SerializeField] private Transform objToPointFrom;


    /// <summary>
    /// Point indicator from a source A to source B.
    /// </summary>
    public void PointIndicator()
    {
        Vector3 dir = objToPointTo.position - objToPointFrom.position;
        dir = dir.normalized;

        // Original direction vector is point to.
        Vector3 origDir = new Vector3(1, 0, 0);


        // Get angle between origDir and dir. (degrees)
        float angle = Vector3.Angle(dir, origDir);

        view.RotateIndicatorTransform(angle);
        // Set portrait rotation 0 relative to parent. (maintain current orientation)
        view.SetPortraitRotationQuaternionIdentity();

        //float angle = Mathf.Acos(Vector3.Dot(dir, origDir));

        // Check if off-screen.
    }

    private void Update()
    {
        // FOR DEBUGGING: TODO: REMOVE LATER:
        if (Input.GetKeyDown(KeyCode.P))
        {
            PointIndicator();
        }
    }
}
