using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

// Attached to targets that have a
public class AggroPossessor : MonoBehaviour
{
    [SerializeField] private LocalEventHandler leh;

    [SerializeField] private float aggroRange;
    [SerializeField] private float aggroReleaseRange;
    [SerializeField, Required] private CircleCollider2D aggroDetecter;
    [SerializeField, Required] private EnemyAIComponent aggroBrain;
    [SerializeField, Required] private bool aggroOnDamaged = true;

    [Header("For Debugging (Don't set values)")]
    [SerializeField, ReadOnly] private GameObject aggroTarget;
    [SerializeField, ReadOnly] private float distFromTarget = -1f;
    [SerializeField, ReadOnly] private bool isAggroed = false;

    private LocalEventBinding<OnDamagedEvent> damagedEventBinding;

    private void Awake()
    {
        Assert.IsNotNull(aggroDetecter, "Aggro Possessor needs way to detect if entity enters aggro range.");
        Assert.IsTrue(aggroReleaseRange > aggroRange, "Aggro release range must be greater than aggro range.");
        Assert.IsNotNull(aggroBrain);

        aggroDetecter.radius = aggroRange;

        if (leh == null)
            leh = InitializerUtil.FindComponentInParent<LocalEventHandler>(this.gameObject);

        damagedEventBinding = new LocalEventBinding<OnDamagedEvent>(OnDamaged);
    }

    private void OnEnable()
    {
        if (damagedEventBinding != null)
        {
            leh.Register(damagedEventBinding);
        }
        
    }

    
    private void OnDisable()
    {
        leh.Unregister(damagedEventBinding);

        //  Disable aggro, if aggroer is not alive.
        isAggroed = false;
        leh?.Call(new OnAggroStatusChangeEvent() { isAggroed = isAggroed });
    }

    // Detects when AggroTarget is in range.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<AggroTarget>() != null)
        {
            if (aggroBrain != null)
            {
                aggroBrain.SetAggroTarget(collision.gameObject);
            }
            
            TriggerAggro(collision.gameObject);
        }
    }

    private void TriggerAggro(GameObject target)
    {
        // If already aggroed, don't trigger aggro again.
        if (isAggroed) return;

        if (aggroBrain != null)
        {
            aggroBrain.SetAggroTarget(target);
        }

        aggroTarget = target;

        // Reset any running AggroCoroutine to avoid multiple coroutines running at the same time.
        StopAllCoroutines();
        isAggroed = true;
        leh.Call(new OnAggroStatusChangeEvent() { isAggroed = isAggroed });
        StartCoroutine(AggroCoroutine());
    }

    // Coroutine to check if aggro target is still in range.
    private IEnumerator AggroCoroutine()
    {
        while (isAggroed)
        {
            if (aggroTarget != null)
            {
                distFromTarget = Vector2.Distance(transform.position, aggroTarget.transform.position);
                if (distFromTarget > aggroReleaseRange)
                {
                    if (aggroBrain != null)
                    {
                        aggroBrain.SetAggroTarget(null);
                    }
                    isAggroed = false;
                    leh.Call(new OnAggroStatusChangeEvent() { isAggroed = isAggroed });
                }
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void OnDamaged(OnDamagedEvent evt)
    {
        if (aggroOnDamaged && !isAggroed)
        {
            TriggerAggro(evt.attackSource.gameObject);
        }
    }

    
}
