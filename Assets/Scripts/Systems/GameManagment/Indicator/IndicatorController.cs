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
    /// Points indicator from a source A (objToPointFrom) to source B (objToPointTo).
    /// </summary>
    public void PointIndicator()
    {
        Vector2 from = Camera.main.WorldToScreenPoint(objToPointFrom.position);
        Vector2 to = Camera.main.WorldToScreenPoint(objToPointTo.position);

        // Target is on-screen?
        if (to.x > 0f && to.x < Screen.width &&
            to.y > 0f && to.y < Screen.height)
        {
            view.Hide();
            return;
        }

        view.Show();

        Vector2 dir = (to - from).normalized;

        // Rotate indicator for correct orientation.
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        view.RotateIndicatorTransform(angle);
        view.SetPortraitRotationQuaternionIdentity();

        // Calculate border based on height and width of rectangle wrapping indicator (after rotation)
        // 0 -> Bottom left
        // 1 -> Top left
        // 2 -> Top right
        // 3 -> Bottom right
        Vector3[] corners = new Vector3[4];
        view.GetViewRect().GetWorldCorners(corners);

        float minX = Mathf.Min(corners[0].x, corners[1].x, corners[2].x, corners[3].x);
        float maxX = Mathf.Max(corners[0].x, corners[1].x, corners[2].x, corners[3].x);

        float minY = Mathf.Min(corners[0].y, corners[1].y, corners[2].y, corners[3].y);
        float maxY = Mathf.Max(corners[0].y, corners[1].y, corners[2].y, corners[3].y);

        float indicatorWidth = maxX - minX;
        float indicatorHeight = maxY - minY;

        Vector2 border = new Vector2(indicatorWidth/2.0f, indicatorHeight/2.0f);

        float left = border.x;
        float right = Screen.width - border.x;
        float bottom = border.y;
        float top = Screen.height - border.y;


        // Find where in border 'dir' will intercept.
        Vector2 hit = IntersectRayWithRect(from, dir, left, right, bottom, top);

        // Move indicator to 'hit' location.
        view.MoveIndicatorTransform(hit);
    }

    /// <summary>
    /// Computes the intersection point between a ray and a screen-space rectangle.
    /// The ray starts at 'origin' and moves in direction 'dir'.
    /// The rectangle is defined by absolute screen coordinates: left, right, bottom, and top.
    /// Returns the first (closest) point where the ray intersects the rectangle boundary.
    /// </summary>
    /// <param name="origin">The screen-space starting point of the ray.</param>
    /// <param name="dir">Normalized direction of the ray.</param>
    /// <param name="left">Left boundary of the rectangle in screen coordinates.</param>
    /// <param name="right">Right boundary of the rectangle in screen coordinates.</param>
    /// <param name="bottom">Bottom boundary of the rectangle in screen coordinates.</param>
    /// <param name="top">Top boundary of the rectangle in screen coordinates.</param>
    /// <returns>The screen-space point where the ray first intersects the rectangle.</returns>

    private Vector2 IntersectRayWithRect(Vector2 origin, Vector2 dir, float left, float right, float bottom, float top)
    {
        bool originInside =
        origin.x >= left && origin.x <= right &&
        origin.y >= bottom && origin.y <= top;

        // If origin is inside rectangle, find closests border in dir
        // If origin is outside rectangle, find furthest border in dir.
        float tBest = originInside ? float.MaxValue : float.MinValue;
        Vector2 hit = origin;

        void TestVertical(float xBorder)
        {
            if (dir.x == 0f) return;

            float t = (xBorder - origin.x) / dir.x;
            if (t <= 0f) return;

            float y = origin.y + dir.y * t;
            if (y < bottom || y > top) return;

            if (originInside)
            {
                if (t < tBest)
                {
                    tBest = t;
                    hit = new Vector2(xBorder, y);
                }
            }
            else
            {
                if (t > tBest)
                {
                    tBest = t;
                    hit = new Vector2(xBorder, y);
                }
            }
        }

        void TestHorizontal(float yBorder)
        {
            if (dir.y == 0f) return;

            float t = (yBorder - origin.y) / dir.y;
            if (t <= 0f) return;

            float x = origin.x + dir.x * t;
            if (x < left || x > right) return;

            if (originInside)
            {
                if (t < tBest)
                {
                    tBest = t;
                    hit = new Vector2(x, yBorder);
                }
            }
            else
            {
                if (t > tBest)
                {
                    tBest = t;
                    hit = new Vector2(x, yBorder);
                }
            }
        }

        // Test all borders
        TestVertical(left);
        TestVertical(right);
        TestHorizontal(bottom);
        TestHorizontal(top);

        return hit;
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
