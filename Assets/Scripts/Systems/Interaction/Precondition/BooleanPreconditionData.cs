using UnityEngine;

[CreateAssetMenu(
    fileName = "New Boolean Precondition",
    menuName = "System/Interaction/Preconditions/Boolean")]
public class BooleanPreconditionData : InteractionPreconditionData
{
    [SerializeField] public ScriptableObjectBoolean Condition;
    [SerializeField] public bool RequiredValue = true;

    public override bool IsMet()
    {
        return Condition != null && Condition.Value == RequiredValue;
    }
}
