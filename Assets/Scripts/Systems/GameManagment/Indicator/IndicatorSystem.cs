using System.Collections.Generic;
using UnityEngine;

public class IndicatorSystem : Singleton<IndicatorSystem>
{
    [SerializeField] private GameObject indicatorPfb;
    [SerializeField] private Transform playerTransform;

    // Parent to spawn indicator under.
    [SerializeField] private Transform indicatorPool;


    private Dictionary<IndicatorController,  GameObject> indicators;

    protected override void Awake()
    {
        base.Awake();
        indicators = new Dictionary<IndicatorController, GameObject>();
    }

    public void CreateIndicator(Transform target)
    {
        ClearDeadIndicators();

        var indicator = Instantiate(indicatorPfb, indicatorPool);
        var indicatorControllerRef = indicator.GetComponent<IndicatorController>();
        indicatorControllerRef.SetSourcePoints(target, playerTransform);
        indicators.Add(indicatorControllerRef, target.gameObject);
    }

    /// <summary>
    /// Function that destroys any indicators that are pointing to non active gameobjects.
    /// </summary>
    private void ClearDeadIndicators()
    {
        // Collect keys that need removal
        List<IndicatorController> removeList = null;

        foreach (var kv in indicators)
        {
            if (!kv.Value.activeSelf)
            {
                (removeList ??= new List<IndicatorController>()).Add(kv.Key);
            }
        }

        if (removeList != null)
        {
            foreach (var key in removeList)
            {
                Destroy(key.gameObject);
                indicators.Remove(key);
            }
        }
    }


    private void FixedUpdate()
    {
        ClearDeadIndicators();
    }
}
