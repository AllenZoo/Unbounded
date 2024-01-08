using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// Attached to targets that have a
public class AggroPossessor : MonoBehaviour
{
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
        Assert.IsNotNull(aggroBrain, "Aggro Posseessor needs AI Component");
        Assert.IsTrue(aggroReleaseRange > aggroRange, "Aggro release range must be greater than aggro range.");
        aggroDetecter.radius = aggroRange;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<AggroTarget>() != null)
        {
            aggroBrain.SetAggroTarget(collision.gameObject);

            aggroTarget = collision.gameObject;
            StopAllCoroutines();
            isAggroed = true;
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
                    aggroBrain.SetAggroTarget(null);
                    isAggroed = false;
                }
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}
