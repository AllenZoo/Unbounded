public class NullSlotCondition : IItemCondition
{
    public bool ConditionMet(Item item)
    {
        // Check if Item data is type of SO_Weapon_Item.
        return item.IsEmpty();
    }
}

