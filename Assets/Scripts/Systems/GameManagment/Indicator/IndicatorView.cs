using UnityEngine;
using UnityEngine.UI;

public class IndicatorView : MonoBehaviour
{
    #region Fields
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
    #endregion

    #region Functions
    public void SetPortraitIconImage(Sprite icon)
    {
        portraitIconImage.sprite = icon;
    }

    /// <summary>
    /// Rotate Indicator Transform by angle (degrees)
    /// </summary>
    /// <param name="angle">angle to rotate by</param>
    /// <param name="resetFirst">whether to reset to no rotation first before applying rotation.</param>
    public void RotateIndicatorTransform(float angle, bool resetFirst = true)
    {
        if (resetFirst)
        {
            indicatorTransform.rotation = Quaternion.identity;
        }

        indicatorTransform.Rotate(0, 0, angle);
    }

    public void MoveIndicatorTransform(Vector3 newPos)
    {
        indicatorTransform.position = newPos;
    }
    public void MoveIndicatorTransformUI(Vector2 anchoredPos)
    {
        RectTransform rectTransform = indicatorTransform.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = anchoredPos;
        }
    }

    public void MoveIndicatorTransform(float x, float y)
    {
        indicatorTransform.position = new Vector3(x, y, 0);
    }

    /// <summary>
    /// Rotate Arrow Transform by angle (degrees)
    /// </summary>
    /// <param name="angle">angle to rotate by</param>
    /// <param name="resetFirst">whether to reset to no rotation first before applying rotation.</param>
    public void RotateArrowTransform(float angle, bool resetFirst = true)
    {
        if (resetFirst)
        {
            arrowTransform.rotation = Quaternion.identity;
        }

        arrowTransform.Rotate(0, 0, angle);
    }

    /// <summary>
    /// Rotate Portrait Transform by angle (degrees)
    /// </summary>
    /// <param name="angle">angle to rotate by</param>
    /// <param name="resetFirst">whether to reset to no rotation (relative to parent) first before applying rotation.</param>
    public void RotatePortraitTransform(float angle, bool resetFirst = true)
    {
        if (resetFirst)
        {
            portraitTransform.rotation = Quaternion.identity;
        }
        portraitTransform.Rotate(0, 0, angle);
    }

    public void SetPortraitRotationQuaternionIdentity()
    {
        portraitTransform.rotation = Quaternion.identity;
    }

    public void Show()
    {
        gameObject.SetActive(true);
        //indicatorCanvas.enabled = true;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        //indicatorCanvas.enabled = false;
    }
    #endregion
}
