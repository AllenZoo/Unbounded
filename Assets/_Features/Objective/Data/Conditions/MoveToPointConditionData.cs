using System;
using UnityEngine;

[CreateAssetMenu(fileName ="new move to point condition", menuName ="System/Objective/Conditions/MoveToPointCondition")]
public class MoveToPointConditionData : ObjectiveConditionData
{
    public Vector3 TargetPosition;
    public float DistanceThreshold = 1f;

    public override IObjectiveCondition CreateInstance()
    {
        return new MoveToPointCondition(TargetPosition, DistanceThreshold);
    }
}

public class MoveToPointCondition : IObjectiveCondition
{
    public event Action OnStateChanged;
    private Objective owner;
    private Vector3 targetPos;
    private float threshold;
    private bool isMet;

    public MoveToPointCondition(Vector3 targetPos, float threshold)
    {
        this.targetPos = targetPos;
        this.threshold = threshold;
    }

    public bool IsMet() => isMet;

    public void Initialize(Objective owner)
    {
        this.owner = owner;
        // In a real system, you might subscribe to a Tick event or use a coroutine from a Mono manager
        // For simplicity, we'll assume there's some periodic check.
    }

    public void Update()
    {
        if (isMet || owner.State != ObjectiveState.ACTIVE) return;

        // Note: In a real project, the Player reference should be managed by a PlayerManager
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && Vector3.Distance(player.transform.position, targetPos) < threshold)
        {
            isMet = true;
            OnStateChanged?.Invoke();
        }
    }

    public void Cleanup() { }
}