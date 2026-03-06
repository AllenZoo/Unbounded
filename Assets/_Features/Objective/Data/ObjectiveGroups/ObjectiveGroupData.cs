using UnityEngine;

[CreateAssetMenu(fileName = "new objective group data", menuName = "System/Objective/Objective Group Data")]
public class ObjectiveGroupData : ScriptableObject
{
    [Tooltip("Name displayed as subtitle for the objective ui")]
    public string GroupName = "";
    public ObjectiveData[] Objectives;
}
