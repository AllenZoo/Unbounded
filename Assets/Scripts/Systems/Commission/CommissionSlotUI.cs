using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Class that handles rendering a commission slot.
/// Also handles the OnClick event for the commission slot.
/// </summary>
public class CommissionSlotUI : MonoBehaviour, IPointerClickHandler
{
    [Required]
    [SerializeField] private TextMeshProUGUI titleText;

    [Required]
    [SerializeField] private Image commissionImageDisplay;

    private Commission commission;

    public void SetCommission(Commission commission)
    {
        this.commission = commission;
        RenderCommssion();
    }

    private void RenderCommssion()
    {
        titleText.text = commission.title;
        commissionImageDisplay.sprite = CommissionAssetGetter.Instance.GetEquipmentSprite(commission.equipmentType);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Handle the click event here
        Debug.Log("CommissionSlotUI clicked!");

        // You can call a method or handle the click event logic here
        HandleClick();
    }

    private void HandleClick()
    {
        EventBus<OnCommissionViewRequestEvent>.Call(new OnCommissionViewRequestEvent { commission = commission });
    }
}
