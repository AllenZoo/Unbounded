using UnityEngine;

public class IndicatorController : MonoBehaviour
{
    [SerializeField] private IndicatorView view;

    [SerializeField] private Transform objToPointTo;
    [SerializeField] private Transform objToPointFrom;
    [SerializeField] private RectTransform canvasRect;

    /// <summary>
    /// Sets the source point variables
    /// </summary>
    /// <param name="objToPointTo"></param>
    /// <param name="objToPointFrom"></param>
    public void SetSourcePoints(Transform objToPointTo, Transform objToPointFrom)
    {
        this.objToPointFrom = objToPointFrom;
        this.objToPointTo = objToPointTo;
    }

    /// <summary>
    /// Points indicator from a source A to source B.
    /// </summary>
    public void PointIndicator()
    {
        Vector3 dir = objToPointTo.position - objToPointFrom.position;
        dir = dir.normalized;

        // Original direction vector is point to.
        Vector3 origDir = new Vector3(1, 0, 0);

        // Get angle between origDir and dir. (degrees)
        //float angle = Vector3.Angle(dir, origDir);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        view.RotateIndicatorTransform(angle);
        // Set portrait rotation 0 relative to parent. (maintain current orientation)
        view.SetPortraitRotationQuaternionIdentity();


        // Border for x and y directions
        Vector2 borderSize = new Vector2(30, 30);
        Vector3 targetPositionScreenPoint = Camera.main.WorldToScreenPoint(objToPointTo.position);

        //bool isOffScreen =
        //    targetPositionScreenPoint.x <= borderSize.x
        //    || targetPositionScreenPoint.x >= Screen.width - borderSize.x
        //    || targetPositionScreenPoint.y <= borderSize.y
        //    || targetPositionScreenPoint.y >= Screen.height - borderSize.y;

        bool isOffScreen =
            targetPositionScreenPoint.x <= 0
            || targetPositionScreenPoint.x >= Screen.width
            || targetPositionScreenPoint.y <= 0
            || targetPositionScreenPoint.y >= Screen.height;

        if (isOffScreen)
        {
            view.Show();
            Vector3 cappedTargetScreenPosition = targetPositionScreenPoint;
            cappedTargetScreenPosition.x = Mathf.Clamp(cappedTargetScreenPosition.x, borderSize.x, Screen.width - borderSize.x);
            cappedTargetScreenPosition.y = Mathf.Clamp(cappedTargetScreenPosition.y, borderSize.x, Screen.height - borderSize.y);

            //Debug.Log($"Clamped Target Screen Position: {cappedTargetScreenPosition}");


            //// convert to UI position
            //RectTransformUtility.ScreenPointToLocalPointInRectangle(
            //    canvasRect,
            //    cappedTargetScreenPosition,
            //    null,
            //    out Vector2 anchoredPos
            //);

            //Debug.Log($"UI Point: {anchoredPos}");

            //view.MoveIndicatorTransformUI(anchoredPos);


            Vector3 pointerWorldPosition = Camera.main.ScreenToWorldPoint(cappedTargetScreenPosition);
            view.MoveIndicatorTransform(cappedTargetScreenPosition);
        } else
        {
            view.Hide();
        }

    }


    public void MoveIndicator(Transform newPos)
    {
        view.transform.position = newPos.position;
    }

    private void Update()
    {
        // FOR DEBUGGING: TODO: REMOVE LATER:
        if (Input.GetKeyDown(KeyCode.P))
        {
            PointIndicator();
        }
        PointIndicator();
    }
}
