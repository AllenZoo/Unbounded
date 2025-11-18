using UnityEngine;
using UnityEngine.UI;

public class IndicatorView : MonoBehaviour
{

    /// <summary>
    /// Reference to indicator transform. Useful for rotation/orientation modifications
    /// </summary>
    [SerializeField] private Transform indicatorTransform;

    /// <summary>
    /// Reference to arrow transform. Useful for rotation/orientation modifications
    /// </summary>
    [SerializeField] private Transform arrowTransform;

    /// <summary>
    /// Reference to portrait transform. Useful for rotation/orientation modifications
    /// </summary>
    [SerializeField] private Transform portraitTransform;

    /// <summary>
    /// Reference to the Image component of portrait icon.
    /// </summary>
    [Tooltip("Reference to the Image component of portrait icon.")]
    [SerializeField] private Image portraitIconImage;

    /// <summary>
    /// The image that will set the value for portraitIconImage
    /// </summary>
    //[Tooltip("The image that will set the value for portraitIconImage")]
    //[SerializeField] private Sprite indicatorIcon;

    [SerializeField] private Canvas indicatorCanvas;


    public void SetPortraitIconImage(Sprite icon)
    {
        portraitIconImage.sprite = icon;
    }

    /// <summary>
    /// Rotate Indicator Transform by angle (degrees)
    /// </summary>
    /// <param name="angle"></param>
    public void RotateIndicatorTransform(float angle)
    {
        indicatorTransform.Rotate(0, 0, angle);
    }

    /// <summary>
    /// Rotate Arrow Transform by angle (degrees)
    /// </summary>
    /// <param name="angle"></param>
    public void RotateArrowTransform(float angle)
    {
        arrowTransform.Rotate(0, 0, angle);
    }

    /// <summary>
    /// Rotate Portrait Transform by angle (degrees)
    /// </summary>
    /// <param name="angle"></param>
    public void RotatePortraitTransform(float angle)
    {
        portraitTransform.Rotate(0, 0, angle);
    }

    public void Show()
    {
        indicatorCanvas.enabled = true;
    }

    public void Hide()
    {
        indicatorCanvas.enabled = false;
    }
}
