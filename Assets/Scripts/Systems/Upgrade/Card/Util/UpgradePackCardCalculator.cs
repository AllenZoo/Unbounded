using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Utility class for determining what cards to pick, given deck data.
/// </summary>
public class UpgradePackCardCalculator
{
    /// <summary>
    /// Randomly takes given 'numCards' from deck.  If numCards > deck.Count, we just return the whole deck.
    /// </summary>
    /// <param name="deck"></param>
    /// <param name="numCards"></param>
    /// <returns></returns>
    public static HashSet<UpgradeCardData> GetCardsFromDeck(UpgradeCardDeckData deck, int numCards)
    {
        HashSet<UpgradeCardData> result = new();

        if (deck == null || deck.Cards == null || deck.Cards.Count == 0)
        {
            Debug.LogWarning("Deck is null or empty. Returning empty result.");
            return result;
        }

        // Clamp numCards to max available
        int countToPick = Mathf.Min(numCards, deck.Cards.Count);

        // Convert HashSet to List so we can randomly access
        List<UpgradeCardData> cardList = deck.Cards.ToList();

        // Shuffle the list
        for (int i = 0; i < cardList.Count; i++)
        {
            int randomIndex = Random.Range(i, cardList.Count);
            (cardList[i], cardList[randomIndex]) = (cardList[randomIndex], cardList[i]);
        }

        // Pick first N cards from shuffled list
        for (int i = 0; i < countToPick; i++)
        {
            result.Add(cardList[i]);
        }

        return result;
    }

    // calculate based on given probality of each rarity appearing.
    // Idea algorithm:
    //    1. assign a percentage to each rarity: 20% common, 10% uncommon, 5% rare, etc.
    //    2. collect all those that 'hit' in a list. (we iterate through all the cards in the deck, and then try to 'hit' it based on percentage. Odds are independant)
    //    3a. we return all the highest rarity cards first (eg. if we request 3 cards and 'hit' 3 common, 1 uncommon, 1 rare --> 1 common, 1 uncommon, 1 rare) first. Any
    //       cards of the same rarity will be shuffled and picked randomly from.
    //
    // ALTERNATE TO 3a.  
    //    3b. we randomly pick the cards from the 'hit' list. (maybe add more odds for higher rarity cards to be picked since so rare.)
    // 
    // CON of 3a.: Common cards may effectively be even rarer than legendary in some scenarios... idk. depends on how many cards there are.
    //public static HashSet<UpgradeCardData> GetCardsFromDeck(UpgradeCardDeckData deck)
    //{
    //    return null;
    //}
}
