using UnityEngine;

public class EquipmentViewConfig
{
    public string Title { get { return title; } private set { } }
    public string WeaponName { get { return weaponName; } private set { } }
    public string WeaponDescription { get { return weaponDescription; } private set { } }

    private string title;
    private string weaponName;
    private string weaponDescription;

    public EquipmentViewConfig()
    {
        title = "<u>Equipment</u>";
        weaponName = "";
        weaponDescription = "";
    }

    public EquipmentViewConfig(string title, string weaponName, string weaponDescription)
    {
        this.title = title;
        this.weaponName = weaponName;
        this.weaponDescription = weaponDescription;
    }
}
