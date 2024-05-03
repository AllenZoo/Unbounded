using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IForger
{
    /// <summary>
    /// Forges upgrades on the given weapon using the given stones. Returns the upgraded weapon.
    /// </summary>
    /// <param name="stones"></param>
    /// <param name="weapon"></param>
    /// <returns></returns>
    public Item Forge(List<Item> stones, Item weapon);
}
public class Forger : IForger
{

    public Item Forge(List<Item> stones, Item weapon)
    {
        return weapon;
    }
}
