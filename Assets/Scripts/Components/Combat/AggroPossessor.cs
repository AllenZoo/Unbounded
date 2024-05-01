using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

// Attached to targets that have a
public class AggroPossessor : MonoBehaviour
{
    [SerializeField] private LocalEventHandler localEventHandler;

    [SerializeField] private float aggroRange;
    [SerializeField] private float aggroReleaseRange;
    [SerializeField] private CircleCollider2D aggroDetecter;
    [SerializeField] private EnemyAIComponent aggroBrain;

    [Header("For Debugging (Don't set values)")]
    [SerializeField] private GameObject aggroTarget;
    [SerializeField] private float distFromTarget = -1f;
    [SerializeField] private bool isAggroed = false;

    private void Awake()
    {
        Assert.IsNotNull(aggroDetecter, "Aggro Possessor needs way to detect if entity enters aggro range.");
        Assert.IsTrue(aggroReleaseRange > aggroRange, "Aggro release range must be greater than aggro range.");
        aggroDetecter.radius = aggroRange;

        if (localEventHandler == null)
        {
            localEventHandler = GetComponentInParent<LocalEventHandler>();
            if (localEventHandler == null)
            {
                Debug.LogError("LocalEventHandler unassigned and not found in parent for object [" + gameObject +
                     "] with root object [" + gameObject.transform.root.name + "] for AggroPossessor.cs");
            }
        }
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
            localEventHandler.Call(new OnAggroStatusChangeEvent() { isAggroed = isAggroed });
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
                    localEventHandler.Call(new OnAggroStatusChangeEvent() { isAggroed = isAggroed });
                }
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    //  Disable aggro, if aggroer is not alive.
    private void OnDisable()
    {
        isAggroed = false;
        localEventHandler.Call(new OnAggroStatusChangeEvent() { isAggroed = isAggroed });
    }
}
