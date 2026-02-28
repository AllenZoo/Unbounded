using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Base class for all card pack components that display a collection of cards and raise a display event.
/// Inherit from this class to implement specific types of card packs (e.g., upgrade cards, starter weapons).
/// </summary>
/// <typeparam name="TCardData">The data type used to represent a single card (e.g., UpgradeCardData).</typeparam>
/// <typeparam name="TDisplayEvent">
/// The event type that will be fired when the card pack is displayed. Must implement <see cref="IEvent"/>.
/// </typeparam>
public abstract class CardPackBase<TCardData, TDisplayEvent> : SerializedMonoBehaviour
    where TDisplayEvent : IEvent
{
    /// <summary>
    /// The collection of cards contained in this card pack.
    /// Serialized for configuration in the Unity Inspector.
    /// </summary>
    [SerializeField]
    protected HashSet<TCardData> cardsInPack = new();

    /// <summary>
    /// Sets the contents of the card pack.
    /// </summary>
    /// <param name="cards">A <see cref="HashSet{TCardData}"/> of card data to store in the pack.</param>
    public void SetCards(HashSet<TCardData> cards)
    {
        cardsInPack = cards;
    }

    /// <summary>
    /// Fires a display event using the current contents of the card pack.
    /// Use this to trigger a UI or gameplay response when the player interacts with the pack.
    /// </summary>
    public void DisplayCards()
    {
        if (cardsInPack.Count <= 0 && Debug.isDebugBuild)
        {
            Debug.LogWarning("No cards to display in card pack.");
        }

        EventBus<TDisplayEvent>.Call(CreateDisplayEvent(cardsInPack));
    }

    /// <summary>
    /// Creates the specific display event using the current card collection.
    /// Must be implemented by subclasses to return the correct event instance.
    /// </summary>
    /// <param name="cards">The set of card data currently in the pack.</param>
    /// <returns>The display event to fire via the event bus.</returns>
    protected abstract TDisplayEvent CreateDisplayEvent(HashSet<TCardData> cards);
}


