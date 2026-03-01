using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // Import DOTween namespace
using Sirenix.OdinInspector;

[RequireComponent(typeof(SpriteRenderer))]
public class ObjectFader : MonoBehaviour
{
    // Reference to the SpriteRenderer component
    private SpriteRenderer spriteRenderer;

    // The desired transparency value
    [SerializeField] private float transparency = 0.7f;

    // Duration of the fade animation
    [SerializeField] private float fadeDuration = 0.2f;

    // Optional ease type for the animation
    [SerializeField] private Ease easeType = Ease.Linear;

    // Current active tween
    private Tween currentTween;

    // This value will be set to true when the object should start fading
    [SerializeField, ReadOnly] private bool doFade = false;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public bool setDoFade(bool shouldFade)
    {
        doFade = shouldFade;
        OnDoFade();
        return doFade;
    }

    private void OnDoFade()
    {
        // Kill any active tween to prevent conflicts
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
        }

        // Create a new tween based on the fade direction
        if (doFade)
        {
            FadeOut();
        }
        else
        {
            FadeIn();
        }
    }

    private void FadeOut()
    {
        // Use DOTween to animate the alpha value to transparency
        currentTween = spriteRenderer.DOFade(transparency, fadeDuration)
            .SetEase(easeType);
    }

    private void FadeIn()
    {
        // Use DOTween to animate the alpha value back to 1
        currentTween = spriteRenderer.DOFade(1f, fadeDuration)
            .SetEase(easeType);
    }

    private void OnDestroy()
    {
        // Clean up any active tweens when the object is destroyed
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
        }
    }
}