using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new behaviour definition data", menuName = "System/Enemy/Behaviour Data")]
public class BehaviourDefinitionData : SerializedScriptableObject
{
    [Required]
    [Tooltip("This is the main identifier and differentiator between behaviours. Should be unique. NOTE: shouldn't be 'empty'.")]
    public string Name; // For debugging purposes, + used for behaviour transitions.

    [OdinSerialize]
    public IAttacker Attacker;

    public EnemyChaseSOBase ChaseBehaviour;
    //[ReadOnly] public EnemyChaseSOBase chaseBehaviourInstance; // We should initialize this ref and not the above, since SO are shared. We need to create new ref using Instantiate.
    public List<AddStatModifier> AddStatModifiers;

    [SerializeField, Tooltip("For keeping track of what attacker and attack objs are being used")]
    [TextArea(3, 10)]
    private string BehaviourDescription = "";
}
