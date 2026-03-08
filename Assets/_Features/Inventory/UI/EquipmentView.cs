using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EquipmentView : MonoBehaviour
{
    [Required, SerializeField] private TextMeshProUGUI titleText;
    [Required, SerializeField] private TextMeshProUGUI weaponNameText;
    [Required, SerializeField] private TextMeshProUGUI weaponDescriptionText;


    private List<SlotUI> slotView;

    public void UpdateView(EquipmentViewConfig config)
    {
        titleText.text = config.Title;
        weaponNameText.text = config.WeaponName;
        weaponDescriptionText.text = config.WeaponDescription;
    }
}
