using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// TODO: eventually split item value component into one for forging costs and another for selling values.
[Serializable]
public class ItemValueComponent : IItemComponent
{
    public int goldValue = 1;

    #region Constructors
    public ItemValueComponent()
    {

    }

    public ItemValueComponent(int goldValue)
    {
        this.goldValue = goldValue;
    }
    #endregion

    public IItemComponent DeepClone()
    {
        return new ItemValueComponent(goldValue);
    }

    #region Equals + Hash
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        ItemValueComponent other = obj as ItemValueComponent;
        return goldValue == other.goldValue;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(goldValue.GetHashCode());
    }
    #endregion
}
