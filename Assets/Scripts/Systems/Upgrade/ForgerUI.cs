using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(ForgerSystem))]
public class ForgerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI previewUpgradeCostText;
    [SerializeField] private TextMeshProUGUI previewAfterText;
    private ForgerSystem system;

    private void Awake()
    {
        system = GetComponent<ForgerSystem>();
    }

    private void Start()
    {
        EventBinding<OnInventoryModifiedEvent> inventoryModifiedBinding = new EventBinding<OnInventoryModifiedEvent>(OnInventoryModified);
        EventBus<OnInventoryModifiedEvent>.Register(inventoryModifiedBinding);
    }

    private void OnInventoryModified(OnInventoryModifiedEvent e)
    {
        UpdateUpgradeCost();
    }

    private void UpdateUpgradeCost()
    {
        previewUpgradeCostText.text = string.Format("Cost: {0} Gold", system.GetForgeCost().ToString());

        previewAfterText.text = string.Format("After Forge: {0} - {1} = {2} Gold",
                                system.GetCurMoney(),
                                system.GetForgeCost(),
                                system.GetAfterForgeResult());
    }



}
