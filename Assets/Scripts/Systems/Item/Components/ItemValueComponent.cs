using System;


// TODO: eventually split item value component into one for forging costs and another for selling values.
[Serializable]
public class ItemValueComponent : IItemComponent
{
    public int goldValue = 1;

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
}
