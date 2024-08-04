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
public class CommissionInfoDisplayUI : PageUI
{
    #region Properties
    [Header("UI Elements")]
    [FoldoutGroup("UI Elements")][Required][SerializeField] private TextMeshProUGUI titleText;
    [FoldoutGroup("UI Elements")][Required][SerializeField] private TextMeshProUGUI descriptionText;
    [FoldoutGroup("UI Elements")][Required][SerializeField] private TextMeshProUGUI rewardText;
    [FoldoutGroup("UI Elements")][Required][SerializeField] private TextMeshProUGUI difficultyText;
    [FoldoutGroup("UI Elements")][Required][SerializeField] private TextMeshProUGUI timeLimitText;
    [FoldoutGroup("UI Elements")][Required][SerializeField] private Image commissionImageDisplay;


    [FoldoutGroup("Data")]
    [Header("Data")]
    [Tooltip("Scriptable Object that holds the current CommissionViewStatus. We modify the status in this class.")]
    [Required][SerializeField] private CommissionViewStatus commissionViewStatus;

    [FoldoutGroup("Data")]
    [Tooltip("For toggling the visability of the CommissionInfoDisplayUI without disabling the GameObject running this script.")]
    [Required][SerializeField] private GameObject wrapper;

    [FoldoutGroup("Data")]
    [Required][SerializeField] private SO_Inventory submitInventory;

    [Header("Stat Tags")]
    [Required][SerializeField] private StatTagUI statTagPfb;
    [Required][SerializeField] private Transform statTagParent;
    private IObjectPooler<StatTagUI> statTagUIPooler;

    private Commission commission;

    private EventBinding<OnCommissionViewInfoRequestEvent> commissionViewReqBinding;
    #endregion

    private void Awake()
    {
        commissionViewReqBinding = new EventBinding<OnCommissionViewInfoRequestEvent>(OnCommissionViewRequest);
        statTagUIPooler = new ConsistentOrderObjectPooler<StatTagUI>(statTagPfb, statTagParent);
    }

    private void OnEnable()
    {
        EventBus<OnCommissionViewInfoRequestEvent>.Register(commissionViewReqBinding);
    }

    private void Start()
    {
        // Test this later to see if it breaks anything.
        // EventBus<OnCommissionViewInfoRequestEvent>.Register(commissionViewReqBinding);
    }

    private void OnDisable()
    {
        // TODO: Figure out better way to do this later.
        //Item submitItem = submitInventory.items[0];

        //if (submitItem != null)
        //{
        //    InventorySystemStorage.Instance.GetSystem(InventoryType.Inventory).AddItem(submitItem);
        //    submitInventory.items[0] = null;
        //}
    }

    public void SetCommission(Commission commission)
    {
        if (this.commission != null)
        {
            // Unsubscribe to previous commission's events.
            this.commission.OnCommissionComplete -= HandleCommissionCompletion;
        }
        this.commission = commission;
        this.commission.OnCommissionComplete += HandleCommissionCompletion;

        Render();
    }

    /// <summary>
    /// A commission completes only when it's submitted through the CommissionInfoDisplayUI.
    /// Thus, we close the CommissionInfoDisplayUI when a commission completes.
    /// </summary>
    /// <param name="commission"></param>
    private void HandleCommissionCompletion(Commission commission)
    {
        ToggleCommissionInfoDisplayVisability(false);
    }

    #region Rendering main CommissionInfoDisplayUI
    private void Render()
    {
        ToggleCommissionInfoDisplayVisability(true);
        titleText.text = commission.title;
        descriptionText.text = commission.description;

        // Convert # to *s for difficulty.
        difficultyText.text = "Difficulty: " +  new string('*', commission.difficulty);
        rewardText.text = "Reward: " + commission.reward + " gold";
        timeLimitText.text = "Time Limit: " + commission.timeLimit + " days";
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
        // When we close the commission info, we want to move any item in the submitInventory back to the inventory.
        //if (!isVisible)
        //{
        //    InventorySystemStorage.Instance.GetSystem(InventoryType.Inventory).AddItem(submitInventory.items[0]);
        //    submitInventory.items[0] = null;
        //}
        // this.gameObject.SetActive(isVisible);
        wrapper.SetActive(isVisible);
        
        if (isVisible)
        {
            Debug.Log("Bringing to front");
            UIOverlayManager.Instance.BringToFront(this);
        }
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
    public void SubmitCommission() => commission.SubmitCommission(submitInventory.items[0]);
    #endregion

}
