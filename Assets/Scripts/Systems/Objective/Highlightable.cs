using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Component class to be attached to objects that can generate a highlight effect.
/// </summary>
public class Highlightable : MonoBehaviour
{
    [SerializeField, Required] private ShaderBank shaderBank;
    [SerializeField, Required] private SpriteRenderer sr;

    private bool keepHighlighting;
    private float maxGlowPower = 1.5f;
    private float minGlowPower = 0f;
    private Material highlight;
    private Tween highlightTween;

    private void Awake()
    {
        Assert.IsNotNull(shaderBank);
        Assert.IsNotNull(sr);
    }

    private void Start()
    {
        highlight = new Material(shaderBank.HighlightMaterial);
    }

    //public void Highlight()
    //{
    //    StartCoroutine(StartHighlight(highlight));
    //}
    //public void StopHighlight()
    //{
    //    keepHighlighting = false;
    //    StopAllCoroutines();
    //    highlight.SetFloat("Highlight", 0);
    //}

    public void Highlight()
    {
        if (highlightTween != null && highlightTween.IsActive())
            return;

        sr.material = new Material(shaderBank.HighlightMaterial); // create new material
        highlight = sr.material;

        // Pulse effect
        highlight.SetFloat("Highlight", 1);
        highlightTween = DOTween.To(
            () => highlight.GetFloat("_GlowPower"),
            x => highlight.SetFloat("_GlowPower", x),
            maxGlowPower,
            1f
        )
        .SetLoops(-1, LoopType.Yoyo)
        .SetEase(Ease.InOutSine);
    }

    public void StopHighlight()
    {
        if (highlightTween != null)
        {
            highlightTween.Kill();
            highlightTween = null;
        }

        // Optional: fade out smoothly
        highlight.DOFloat(0f, "_GlowPower", 0.3f);
        highlight.SetFloat("Highlight", 0);
    }



    private IEnumerator StartHighlight(Material highlight)
    {
        sr.material = highlight;
        highlight.SetFloat("Highlight", 1); //TODO: check if this works even though property is bool.
        keepHighlighting = true;

        while (keepHighlighting)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1f);
    }

    // TODO: for debugging. Remove afterwards.
    private bool toggle = false;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!toggle)
            {
                Highlight();
            } else
            {
                StopHighlight();
            }
            toggle = !toggle;
        }
    }
}
