public class EquipmentSlotCondition : IItemCondition
{
    public bool ConditionMet(Item item)
    {
        if (item == null || item.IsEmpty())
        {
            // 'No' item meets all conditions.
            // (eg. We can have an empty upgrade slot)
            return true;
        }

        return item.HasComponent<ItemEquipmentComponent>();
    }
}