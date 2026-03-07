using Sirenix.OdinInspector;
using UnityEngine;

public abstract class ObjectiveConditionData : SerializedScriptableObject
{
    public abstract IObjectiveCondition CreateInstance();
}
