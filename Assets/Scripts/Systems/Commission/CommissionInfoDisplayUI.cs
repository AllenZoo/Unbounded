using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class that handles rendering the proper info on the Commission Info Display UI.
/// </summary>
public class CommissionInfoDisplayUI : MonoBehaviour
{
    [Required][SerializeField] private TextMeshProUGUI titleText;
    [Required][SerializeField] private TextMeshProUGUI descriptionText;
    [Required][SerializeField] private TextMeshProUGUI rewardText;
    [Required][SerializeField] private TextMeshProUGUI timeLimitText;
    [Required][SerializeField] private Image commissionImageDisplay;

    // TODO: Add UI to display Required Stats

    private Commission commission;


    private EventBinding<OnCommissionViewRequestEvent> commissionViewReqBinding;

    private void Awake()
    {
        commissionViewReqBinding = new EventBinding<OnCommissionViewRequestEvent>(OnCommissionViewRequest);
    }

    private void OnEnable()
    {
        EventBus<OnCommissionViewRequestEvent>.Register(commissionViewReqBinding);
    }

    private void OnDisable()
    {
        EventBus<OnCommissionViewRequestEvent>.Unregister(commissionViewReqBinding);
    }

    public void SetCommission(Commission commission)
    {
        this.commission = commission;
        Render();
    }

    private void Render()
    {
        ToggleCommissionInfoDisplayVisability(true);
        titleText.text = commission.title;
        descriptionText.text = commission.description;
        rewardText.text = "Reward: " + commission.reward;
        timeLimitText.text = "Time: " + commission.timeLimit;
        commissionImageDisplay.sprite = CommissionAssetGetter.Instance.GetEquipmentSprite(commission.equipmentType);
    }

    public void ToggleCommissionInfoDisplayVisability(bool isVisible)
    {
        this.gameObject.SetActive(isVisible);
    }

    private void OnCommissionViewRequest(OnCommissionViewRequestEvent e)
    {
        SetCommission(e.commission);
    }
}
