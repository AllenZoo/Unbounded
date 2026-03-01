using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropRateCalculator
{
    /// <summary>
    /// Returns a random item from the drop rates.
    /// </summary>
    /// <param name="dropRates"></param>
    /// <returns>An Item or null (empty item)</returns>
    public static Item GetItemFromDropRate(DropRates dropRates)
    {
        float totalRate = dropRates.emptyDropRate;
        foreach (DropRate dropRate in dropRates.data.dropRates)
        {
            totalRate += dropRate.rate;
        }
        float random = Random.Range(0, totalRate);
        float currentRate = 0;
        foreach (DropRate dropRate in dropRates.data.dropRates)
        {
            currentRate += dropRate.rate;
            if (random <= currentRate)
            {
                if (dropRate.item.Data == null)
                {
                    return null;
                }
                return dropRate.item;
            }
        }
        // Return empty item (null).
        return null;
    }

    /// <summary>
    /// Returns a list of random items from the drop rates.
    /// </summary>
    /// <param name="dropRates"></param>
    /// <param name="numItems"></param>
    /// <returns>A list of size numItems containing all dropped items (could include null items).
    /// The list will be sorted such that non null items are in the front of the list.</returns>
    public static List<Item> GetItemsFromDropRate(DropRates dropRates, int numItems)
    {
        List<Item> items = new List<Item>();
        for (int i = 0; i < numItems; i++)
        {
            Item itemDrop = GetItemFromDropRate(dropRates);
            items.Add(itemDrop);
        }

        // Sort the list such that non null items are in the front of the list.
        items.Sort((x, y) =>
        {
            if (x == null && y == null) return 0;
            if (x == null) return 1;
            if (y == null) return -1;
            return 0;
        });

        return items;
    }
}
