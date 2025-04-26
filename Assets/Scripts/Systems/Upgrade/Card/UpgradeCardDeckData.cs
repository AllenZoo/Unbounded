using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new upgrade card deck", menuName = "System/Upgrade/UpgradeCardDeck")]
public class UpgradeCardDeckData : SerializedScriptableObject
{
    [OdinSerialize, SerializeField]
    public HashSet<UpgradeCardData> Cards = new();
}
