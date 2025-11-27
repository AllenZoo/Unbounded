using Sirenix.OdinInspector;
using UnityEngine;

public class CircleScaler : MonoBehaviour
{
    [Required, SerializeField] private GameObject circleObj;

    public void SetCircleRadius(float radius)
    {
        circleObj.transform.localScale = new Vector3(radius * 2, radius * 2, 1);
    }
}
