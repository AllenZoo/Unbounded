using UnityEngine;

public abstract class InteractionPreconditionData : ScriptableObject
{
    [TextArea]
    [SerializeField] protected string failureMessage;

    public string FailureMessage => failureMessage;

    public abstract bool IsMet();
}

