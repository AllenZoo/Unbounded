using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Attached to components that dissolve on death
/// </summary>
public class DissolveOnDeath : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Required] private ShaderBank shaderBank;
    [SerializeField, Required] private LocalEventHandler leh;
    [SerializeField, Required] private SpriteRenderer sr;
    [SerializeField] private SoundType onDeathSfx = SoundType.None;

    [Header("Properties")]
    [Tooltip("Time to delay the dissolve animation for.")]
    [SerializeField] private float DelayTime = 0f;

    private LocalEventBinding<OnDeathEvent> OnDeathBinding;
    private LocalEventBinding<OnRespawnEvent> OnRespawnBinding;
    [SerializeField, ReadOnly] private Material originalMaterial; // serialized for debugging purposes.
    private Coroutine dissolveCoroutine;

    #region Unity Lifecycle Methods
    private void Awake()
    {
        if (leh == null) leh = InitializerUtil.FindComponentInParent<LocalEventHandler>(gameObject);

        Assert.IsNotNull(shaderBank);
        Assert.IsNotNull(leh);
        Assert.IsNotNull(sr);
        originalMaterial = sr.material;
        // If we captured a dissolve material as original, try to find a better one
        if (originalMaterial != null && (originalMaterial.name.Contains("Dissolve") || originalMaterial.HasProperty("_Fade")))
        {
             Material defaultSpriteMat = AssetDatabase.GetBuiltinExtraResource<Material>("Sprites-Default.mat");
             if (defaultSpriteMat != null) originalMaterial = defaultSpriteMat;
        }
    }

    private void OnEnable()
    {
        if (OnDeathBinding == null) OnDeathBinding = new LocalEventBinding<OnDeathEvent>(OnDeathEvent);
        leh.Register(OnDeathBinding);

        if (OnRespawnBinding == null) OnRespawnBinding = new LocalEventBinding<OnRespawnEvent>(OnRespawn);
        leh.Register(OnRespawnBinding);
    }

    private void OnDestroy()
    {
        leh.Unregister(OnDeathBinding);
        leh.Unregister(OnRespawnBinding);
    }

    private void OnDisable()
    {
        leh.Unregister(OnDeathBinding);
        leh.Unregister(OnRespawnBinding);
    }
    #endregion

    private void OnDeathEvent(OnDeathEvent e)
    {
        // Set the sprite material to dissolve.
        Material dissolve = new Material(shaderBank.DissolveMaterial);
        AudioManager.PlaySound(onDeathSfx, 1);
        
        if (dissolveCoroutine != null) StopCoroutine(dissolveCoroutine);
        dissolveCoroutine = StartCoroutine(StartDissolving(dissolve));
    }

    private void OnRespawn(OnRespawnEvent e)
    {
        if (dissolveCoroutine != null)
        {
            StopCoroutine(dissolveCoroutine);
            dissolveCoroutine = null;
        }

        // Restore original material and ensure it's fully visible
        sr.material = originalMaterial;
        if (sr.material.HasProperty("_Fade"))
        {
            sr.material.SetFloat("_Fade", 1f);
        }
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
    }

    private IEnumerator StartDissolving(Material dissolve)
    {
        yield return new WaitForSeconds(DelayTime);
        sr.material = dissolve;
        sr.color = Color.white; // Ensure visibility by overriding black tint from EffectsController

        // Ensure _Fade starts at 1 (fully visible)
        dissolve.SetFloat("_Fade", 1f);

        // Tween the _Fade property from 1 to 0 over 1 second
        dissolve.DOFloat(0f, "_Fade", 1f)
                .SetEase(Ease.InOutSine);

        // Wait for dissolve to finish
        yield return new WaitForSeconds(1f);
        dissolveCoroutine = null;
    }

}
