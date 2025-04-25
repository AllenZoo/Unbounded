
/* Unmerged change from project 'Assembly-CSharp.Player'
Before:
 using System.Collections;
After:
using System.Collections;
*/
using UnityEngine;

// Script intended for objects that need to follow mouse.
// Attach to objects that follow mouse.
public class MouseHover : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private Vector2 offset;
    [SerializeField] private Pivot pivot = Pivot.BOTTOM_LEFT;

    private RectTransform rectTransform;
    private Camera mainCamera;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = rectTransform.root.GetComponent<Canvas>();
        UpdateCameraRef();

        EventBinding<OnSceneLoadRequestFinish> sceneLoadFinishBinding = new EventBinding<OnSceneLoadRequestFinish>(UpdateCameraRef);
        EventBus<OnSceneLoadRequestFinish>.Register(sceneLoadFinishBinding);
    }

    private void Update()
    {
        if (mainCamera == null) return;

        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)canvas.transform,
            Input.mousePosition,
            canvas.worldCamera,
            out position
        );

        // Adjust the position based on the canvas pivot
        switch (pivot)
        {
            case Pivot.BOTTOM_LEFT:
                position += new Vector2(rectTransform.rect.width / 2, rectTransform.rect.height / 2);
                break;
            case Pivot.BOTTOM_RIGHT:
                position += new Vector2(-rectTransform.rect.width / 2, rectTransform.rect.height / 2);
                break;
            case Pivot.TOP_LEFT:
                position += new Vector2(rectTransform.rect.width / 2, -rectTransform.rect.height / 2);
                break;
            case Pivot.TOP_RIGHT:
                position += new Vector2(-rectTransform.rect.width / 2, -rectTransform.rect.height / 2);
                break;
            case Pivot.MIDDLE_LEFT:
                position += new Vector2(rectTransform.rect.width / 2, 0);
                break;
            case Pivot.MIDDLE_RIGHT:
                position += new Vector2(-rectTransform.rect.width / 2, 0);
                break;
            case Pivot.BOTTOM_CENTER:
                position += new Vector2(0, rectTransform.rect.height / 2);
                break;
            case Pivot.TOP_CENTER:
                position += new Vector2(0, -rectTransform.rect.height / 2);
                break;
            case Pivot.MIDDLE_CENTER:
                // Default pivot is MIDDLE_CENTER
                break;
        }

        Vector3 targetPosition = canvas.transform.TransformPoint(position) + (Vector3)offset;

        // Clamp the position within the Canvas or Camera boundaries
        Vector3 clampedPosition = ClampPositionToCanvas(targetPosition);

        transform.position = clampedPosition;
    }

    private void OnEnable()
    {
        UpdateCameraRef();
    }

    private void OnDisable()
    {
        // Move to the bottom left corner of the screen.
        transform.position = new Vector3(-1000, -1000, 0);
    }

    public void Toggle(bool turnOn)
    {
        gameObject.SetActive(turnOn);
    }

    // Clamp the position to be within the Canvas or Camera boundaries
    private Vector3 ClampPositionToCanvas(Vector3 targetPosition)
    {
        if (mainCamera == null) return targetPosition; // Note: redundant guard and shouldn't be triggered since we guard this case in Update().

        if (canvas.renderMode == RenderMode.WorldSpace)
        {
            // Canvas is in World Space
            return targetPosition;
        }
        else
        {
            // TODO: fix logic so it works.
            // Canvas is in Screen Space Overlay or Screen Space Camera
            Vector3 clampedPosition = targetPosition;

            Vector3 minPosition = mainCamera.WorldToScreenPoint(canvas.GetComponent<RectTransform>().rect.min);
            Vector3 maxPosition = mainCamera.WorldToScreenPoint(canvas.GetComponent<RectTransform>().rect.max);

            clampedPosition.x = Mathf.Clamp(clampedPosition.x, minPosition.x, maxPosition.x);
            clampedPosition.y = Mathf.Clamp(clampedPosition.y, minPosition.y, maxPosition.y);

            return clampedPosition;
        }
    }

    private void UpdateCameraRef()
    {
        if (Camera.main != null)
        {
            mainCamera = Camera.main;
        }
    }
}