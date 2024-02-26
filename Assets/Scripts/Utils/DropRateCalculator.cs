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
    /// <returns></returns>
    public static List<Item> GetItemsFromDropRate(DropRates dropRates, int numItems)
    {
        List<Item> items = new List<Item>();
        for (int i = 0; i < numItems; i++)
        {
            items.Add(GetItemFromDropRate(dropRates));
        }
        return items;
    }
}
