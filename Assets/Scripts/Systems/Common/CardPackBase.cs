using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class CardPackBase<TCardData, TDisplayEvent> : SerializedMonoBehaviour
    where TDisplayEvent : IEvent
{
    [SerializeField]
    protected HashSet<TCardData> cardsInPack = new();

    public void SetCards(HashSet<TCardData> cards)
    {
        cardsInPack = cards;
    }

    public void DisplayCards()
    {
        if (cardsInPack.Count <= 0 && Debug.isDebugBuild)
        {
            Debug.LogWarning("No cards to display in card pack.");
        }

        EventBus<TDisplayEvent>.Call(CreateDisplayEvent(cardsInPack));
    }

    protected abstract TDisplayEvent CreateDisplayEvent(HashSet<TCardData> cards);
}

