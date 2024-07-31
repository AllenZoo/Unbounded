using Sirenix.OdinInspector;
using System;
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

    [Tooltip("Scriptable Object that holds the current CommissionViewStatus. We modify the status in this class.")]
    [Required][SerializeField] private CommissionViewStatus commissionViewStatus;

    // TODO: Add UI to display Required Stats
    [Required][SerializeField] private StatTagUI statTagPfb;
    [Required][SerializeField] private Transform statTagParent;
    private IObjectPooler<StatTagUI> statTagUIPooler;

    private Commission commission;

    private EventBinding<OnCommissionViewInfoRequestEvent> commissionViewReqBinding;

    private void Awake()
    {
        commissionViewReqBinding = new EventBinding<OnCommissionViewInfoRequestEvent>(OnCommissionViewRequest);
        statTagUIPooler = new ConsistentOrderObjectPooler<StatTagUI>(statTagPfb, statTagParent);
    }

    private void OnEnable()
    {
        EventBus<OnCommissionViewInfoRequestEvent>.Register(commissionViewReqBinding);
    }

    public void SetCommission(Commission commission)
    {
        this.commission = commission;
        Render();
    }

    #region Rendering main CommissionInfoDisplayUI
    private void Render()
    {
        ToggleCommissionInfoDisplayVisability(true);
        titleText.text = commission.title;
        descriptionText.text = commission.description;
        rewardText.text = "Reward: " + commission.reward;
        timeLimitText.text = "Time: " + commission.timeLimit;
        commissionImageDisplay.sprite = CommissionAssetGetter.Instance.GetEquipmentSprite(commission.equipmentType);

        // TODO: refactor this eventually
        if (commission.commissionStatus == CommissionStatus.ACTIVE) SetViewStatusActive();
        else if (commission.commissionStatus == CommissionStatus.PENDING) SetViewStatusPending();

        statTagUIPooler.ResetObjects();
        foreach (var stat in commission.statRequirements)
        {
            var statTag = statTagUIPooler.GetObject();
            var statTuple = new Tuple<Stat, int>(stat.Key, stat.Value);
            statTag.SetStat(statTuple);
        }
    }
    private void ToggleCommissionInfoDisplayVisability(bool isVisible)
    {
        this.gameObject.SetActive(isVisible);
    }
    private void OnCommissionViewRequest(OnCommissionViewInfoRequestEvent e)
    {
        SetCommission(e.commission);
    }
    #endregion

    #region For View Status + Commission Status Modification via Buttons
    public void SetViewStatusActive() => commissionViewStatus.SetStatus(CommissionViewStatusType.ACTIVE);

    public void SetViewStatusPending() => commissionViewStatus.SetStatus(CommissionViewStatusType.PENDING);

    public void AcceptCommission()
    {
        commission.StartCommission();
        ToggleCommissionInfoDisplayVisability(false);
    }

    public void RejectCommission() => ToggleCommissionInfoDisplayVisability(false);
    #endregion

}
