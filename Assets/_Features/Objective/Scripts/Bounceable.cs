using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component script attached to objects that want to move up and down in a loop. (eg. Objective Arrow)
/// </summary>
public class Bounceable : MonoBehaviour
{
    [Tooltip("How much the object should move.")]
    [SerializeField] private float movementRange = 0.5f;
    [SerializeField] private float bounceDuration = 1f;
    [SerializeField] private bool bounceOnEnable = true;

    private Tween bounceTween;
    private Vector3 originalLocalPos;

    private void Awake()
    {
        originalLocalPos = transform.localPosition;
    }

    private void OnEnable()
    {
        if (bounceOnEnable)
        {
            StartBounce();
        }
    }

    private void OnDisable()
    {
        StopBounce();
    }

    public void StartBounce()
    {
        StopBounce(); // Kill any existing tween

        transform.localPosition = originalLocalPos;

        bounceTween = transform
            .DOLocalMove(originalLocalPos + Vector3.up * movementRange, bounceDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .SetUpdate(UpdateType.Normal, true); // Use independent time (optional for UI/consistent timing)
    }

    public void StopBounce()
    {
        if (bounceTween != null)
        {
            bounceTween.Kill();
            bounceTween = null;
        }

        transform.localPosition = originalLocalPos;
    }
}
