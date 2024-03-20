using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

// Attached to targets that have a
public class AggroPossessor : MonoBehaviour
{
    // isAggroed?
    public event Action<bool> OnAggroStatusChange;

    // TODO: for testing
    public UnityEvent OnAggro;

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
        // Assert.IsNotNull(aggroBrain, "Aggro Posseessor needs AI Component");
        Assert.IsTrue(aggroReleaseRange > aggroRange, "Aggro release range must be greater than aggro range.");
        aggroDetecter.radius = aggroRange;
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
            OnAggroStatusChange?.Invoke(true);
            OnAggro.Invoke();
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
                    OnAggroStatusChange?.Invoke(false);
                } else
                {
                    // Keep throwing Aggro Event
                    OnAggro.Invoke();
                }
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    //  Disable aggro, if aggroer is not alive.
    private void OnDisable()
    {
        isAggroed = false;
        OnAggroStatusChange?.Invoke(false);
    }
}
