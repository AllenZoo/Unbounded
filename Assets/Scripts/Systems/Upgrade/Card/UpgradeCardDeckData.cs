using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new upgrade card deck", menuName = "System/Upgrade/UpgradeCardDeck")]
public class UpgradeCardDeckData : SerializedScriptableObject
{
    [OdinSerialize]
    [Title("Upgrade Card Deck")]
    [ListDrawerSettings(ShowIndexLabels = true)]
    [ValidateInput(nameof(NoDuplicates), "Duplicate cards are not allowed")]
    private List<UpgradeCardData> cards = new();

    // Runtime-safe view
    private HashSet<UpgradeCardData> cardSet;

    public IReadOnlyCollection<UpgradeCardData> Cards
    {
        get
        {
            if (cardSet == null)
                cardSet = new HashSet<UpgradeCardData>(cards);

            return cardSet;
        }
    }

    private bool NoDuplicates(List<UpgradeCardData> list)
    {
        return list.Count == new HashSet<UpgradeCardData>(list).Count;
    }

}
