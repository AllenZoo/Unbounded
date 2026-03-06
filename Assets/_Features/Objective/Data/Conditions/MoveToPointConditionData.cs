using System;
using UnityEngine;

[CreateAssetMenu(fileName ="new move to point condition", menuName ="System/Objective/Conditions/MoveToPointCondition")]
public class MoveToPointConditionData : ObjectiveConditionData
{
    public TransformContext transformContext;
    public float DistanceThreshold = 1f;

    public override IObjectiveCondition CreateInstance()
    {
        return new MoveToPointCondition(this);
    }
}

public class MoveToPointCondition : IObjectiveCondition
{
    public event Action OnStateChanged;
    private Objective owner;
    private MoveToPointConditionData data;
    private bool isMet;
    private Transform targetTransform;

    public MoveToPointCondition(MoveToPointConditionData data)
    {
        this.data = data;
    }

    public bool IsMet() => isMet;

    public void Initialize(Objective owner)
    {
        this.owner = owner;
        targetTransform = data.transformContext.GetContext().Value;
        if (targetTransform == null)
        {
            // If not loaded yet, subscribe to context changes
            data.transformContext.OnContextChanged += HandleContextChanged;
        }
    }

    public void Update(float deltaTime)
    {
        if (targetTransform == null) return;
        if (isMet || owner.State != ObjectiveState.ACTIVE) return;
        

        var player = PlayerSingleton.Instance.gameObject;
        if (player != null && Vector3.Distance(player.transform.position, targetTransform.position) < data.DistanceThreshold)
        {
            isMet = true;
            OnStateChanged?.Invoke();
        }
    }

    public void Cleanup() {
        data.transformContext.OnContextChanged -= HandleContextChanged;
    }

    private void HandleContextChanged(Transform newTransform)
    {
        targetTransform = newTransform;
        if (targetTransform == null)
        {
            Debug.LogError("MoveToPointCondition: No valid transform found in context.");
            return;
        }
    }
}