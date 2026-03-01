using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class CircleScaler : MonoBehaviour
{
    [Required, SerializeField] private GameObject circleObj;

    public void SetCircleRadius(float radius)
    {
        circleObj.transform.localScale = new Vector3(radius * 2, radius * 2, 1);
    }

    public void TransitionCircleRadius(float startRadius, float endRadius, float transitionTime)
    {
        circleObj.transform.localScale = new Vector3(startRadius * 2, startRadius * 2, 1);
        circleObj.transform
            .DOScale(new Vector3(endRadius * 2, endRadius * 2, 1), transitionTime);
    }
}
