using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BossOnDeathSetter : MonoBehaviour
{
    [SerializeField] private List<SerializableObjectBoolean> booleanStatesToSetTrueOnDeath;
    [SerializeField] private LocalEventHandler leh;

    private LocalEventBinding<OnDeathEvent> onDeathBinding;

    private void Awake()
    {
        if (leh == null) leh = InitializerUtil.FindComponentInParent<LocalEventHandler>(gameObject);
        Assert.IsNotNull(leh);
        onDeathBinding = new LocalEventBinding<OnDeathEvent>(OnDeathEvent);
    }

    private void Start()
    {
        leh.Register(onDeathBinding);
    }

    private void OnDisable()
    {
        leh.Unregister(onDeathBinding);
    }

    private void OnDestroy()
    {
        leh.Unregister(onDeathBinding);
    }

    private void OnDeathEvent(OnDeathEvent e)
    {
        foreach (var boolToSet in booleanStatesToSetTrueOnDeath)
        {
            boolToSet.Set(true);
        }
    }

}
