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

    [Header("For Debugging (Don't set values)")]
    [SerializeField, ReadOnly] private GameObject aggroTarget;
    [SerializeField, ReadOnly] private float distFromTarget = -1f;
    [SerializeField, ReadOnly] private bool isAggroed = false;

    private void Awake()
    {
        Assert.IsNotNull(aggroDetecter, "Aggro Possessor needs way to detect if entity enters aggro range.");
        Assert.IsTrue(aggroReleaseRange > aggroRange, "Aggro release range must be greater than aggro range.");
        Assert.IsNotNull(aggroBrain);

        aggroDetecter.radius = aggroRange;

        if (leh == null)
            leh = InitializerUtil.FindComponentInParent<LocalEventHandler>(this.gameObject);
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
            

            aggroTarget = collision.gameObject;
            StopAllCoroutines();
            isAggroed = true;
            leh.Call(new OnAggroStatusChangeEvent() { isAggroed = isAggroed });
            StartCoroutine(AggroCoroutine());
        }
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

    //  Disable aggro, if aggroer is not alive.
    private void OnDisable()
    {
        isAggroed = false;
        leh?.Call(new OnAggroStatusChangeEvent() { isAggroed = isAggroed });
    }
}
