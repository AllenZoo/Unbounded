using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IForger
{
    /// <summary>
    /// Forges upgrades on the given equipment using the given stones. Returns the upgraded equipment.
    /// </summary>
    /// <param name="stones"></param>
    /// <param name="equipment"></param>
    /// <returns></returns>
    public Item Forge(List<Item> stones, Item equipment);
}
public class Forger : IForger
{

    public Forger()
    {
    }

    public Item Forge(List<Item> stones, Item equipment)
    {
        

        return equipment;
    }
}
