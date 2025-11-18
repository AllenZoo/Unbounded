using UnityEngine;

public class BossIndicator : MonoBehaviour
{
    public Transform boss;
    public Transform player;
    public RectTransform arrowUI;
    public Camera cam;

    public float borderOffset = 60f;

    void Update()
    {
        Vector3 screenPos = cam.WorldToScreenPoint(boss.position);

        bool isOffscreen =
            screenPos.z < 0 ||
            screenPos.x < 0 || screenPos.x > Screen.width ||
            screenPos.y < 0 || screenPos.y > Screen.height;

        arrowUI.gameObject.SetActive(isOffscreen);
        if (!isOffscreen) return;

        // Direction
        Vector3 dir = (boss.position - player.position).normalized;

        // Rotate arrow
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        arrowUI.rotation = Quaternion.Euler(0, 0, angle - 90f);

        // Clamp position
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        Vector3 screenPosFromCenter = screenPos - screenCenter;

        Vector3 clampedPos = screenCenter +
            Vector3.ClampMagnitude(screenPosFromCenter, (Screen.height / 2f) - borderOffset);

        arrowUI.position = clampedPos;
    }
}
