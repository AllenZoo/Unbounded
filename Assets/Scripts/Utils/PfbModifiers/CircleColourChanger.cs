using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class CircleColourChanger : MonoBehaviour
{
    [Required, SerializeField] SpriteRenderer circleRenderer;

    public void ChangeColour(Color newColour, float duration = 0.2f)
    {
        if (circleRenderer != null)
        {
            circleRenderer.DOColor(newColour, duration);
        }
    }

    public Tween TransitionColour(Color startColour, Color endColour, float duration = 0.2f)
    {
        if (circleRenderer != null)
        {
            circleRenderer.color = startColour;
            return circleRenderer.DOColor(endColour, duration);
        }

        return null;
    }
}
