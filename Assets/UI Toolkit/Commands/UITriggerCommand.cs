using UnityEngine;

public abstract class UITriggerCommand : ScriptableObject
{
    public abstract void Execute();
    public abstract void Undo();
}
