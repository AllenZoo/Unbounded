public class WeaponSlotCondition : IItemCondition
{
    public bool ConditionMet(Item item)
    {
        if (item == null || item.IsEmpty())
        {
            // 'No' item meets all conditions.
            // (We can have an empty weapon slot)
            return true;
        }

        // Check if Item data is type of SO_Weapon_Item.
        return item.HasComponent<ItemAttackContainerComponent>();
    }
}
