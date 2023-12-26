using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ObjectFader : MonoBehaviour
{
    // Reference to the SpriteRenderer component
    private SpriteRenderer spriteRenderer;

    // The desired transparency value
    [SerializeField] private float transparency = 0.5f;

    // This value will be set to true when the object should start fading
    [SerializeField] private bool doFade = false;

    [SerializeField] private float fadeSpeed = 0.5f;

    // Start is called before the first frame update
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
        if (doFade) {
            StartCoroutine(FadeOut());
        }
        else {
            StartCoroutine(FadeIn());
        }
    }

    private IEnumerator FadeOut()
    {
        while (spriteRenderer.color.a > transparency)
        {
            Color color = spriteRenderer.color;
            color.a -= fadeSpeed * Time.deltaTime;
            spriteRenderer.color = color;
            yield return null;
        }
    }

    private IEnumerator FadeIn()
    {
        while (spriteRenderer.color.a < 1)
        {
            Color color = spriteRenderer.color;
            color.a += fadeSpeed * Time.deltaTime;
            spriteRenderer.color = color;
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        OnDoFade();
    }
}
