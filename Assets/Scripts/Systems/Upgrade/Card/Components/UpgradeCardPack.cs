using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MonoBehaviour class attached to UpgradeCardPack pfb.
/// </summary>
public class UpgradeCardPack : SerializedMonoBehaviour
{
    [SerializeField] private HashSet<UpgradeCardData> cardsInPack = new HashSet<UpgradeCardData>();

    /// <summary>
    /// Fires an event to display UpgradeCardPack in UI. This function should be hooked up in the pfb to UnityEvent 'OnInteract' via inspector to
    /// PromptInteractor component.
    /// </summary>
    public void DisplayCards()
    {
        if (cardsInPack.Count <= 0 && Debug.isDebugBuild)
        {
            Debug.LogError("Requesting to display cards with no cards in pack.");
            //return;
        }

        EventBus<OnDisplayUpgradeCardsRequest>.Call(new OnDisplayUpgradeCardsRequest() { upgradeCards = cardsInPack });
    }

    public void SetCards(HashSet<UpgradeCardData> cards)
    {
        this.cardsInPack = cards;
    }

}
