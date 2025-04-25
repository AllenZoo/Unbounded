using Sirenix.OdinInspector;

public class ItemEquipmentComponent : IItemComponent
{
    [Required]
    public EquipmentType equipmentType;


    public ItemEquipmentComponent(EquipmentType equipmentType)
    {
        this.equipmentType = equipmentType;
    }
}
