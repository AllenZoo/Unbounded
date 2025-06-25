using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
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

    [Header("Properties")]
    [Tooltip("Time to delay the dissolve animation for.")]
    [SerializeField] private float DelayTime = 1f;

    private LocalEventBinding<OnDeathEvent> OnDeathBinding;

    #region Unity Lifecycle Methods
    private void Awake()
    {
        if (leh == null) leh = InitializerUtil.FindComponentInParent<LocalEventHandler>(gameObject);

        Assert.IsNotNull(shaderBank);
        Assert.IsNotNull(leh);
        Assert.IsNotNull(sr);
    }

    private void Start()
    {
        OnDeathBinding = new LocalEventBinding<OnDeathEvent>(OnDeathEvent);
        leh.Register(OnDeathBinding);
    }

    private void OnDestroy()
    {
        leh.Unregister(OnDeathBinding);
    }

    private void OnDisable()
    {
        leh.Unregister(OnDeathBinding);
    }
    #endregion

    private void OnDeathEvent(OnDeathEvent e)
    {
        // Set the sprite material to dissolve.
        Material dissolve = new Material(shaderBank.DissolveMaterial);
        StartCoroutine(StartDissolving(dissolve));
    }

    private IEnumerator StartDissolving(Material dissolve)
    {
        yield return new WaitForSeconds(DelayTime);
        sr.material = dissolve;

        // Ensure _Fade starts at 1 (fully visible)
        dissolve.SetFloat("_Fade", 1f);

        // Tween the _Fade property from 1 to 0 over 1 second
        dissolve.DOFloat(0f, "_Fade", 1f)
                .SetEase(Ease.InOutSine);

        // Wait for dissolve to finish
        yield return new WaitForSeconds(1f);
    }

}
