using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

/// TODO: remove this class or rename it. It does too much stuff rn. Violates SRP.
/// <summary>
/// Handles managing commissions on the commission board.
/// TODO-OPT: refactor this responsibility somehwere else if this class gets too messy: 
///     Also handles the validation of commission submissions.
/// </summary>
/// 
public class CommissionBoard : MonoBehaviour
{
    // CommissionBoard AKA CommissionStateHandler. In charge of 
    //    1. Generating commission sets (3 commissions for player to choose from)
    //    2. Validating commission submissions.

    [Tooltip("Context to populate Commissions with.")]
    [SerializeField, Required] private CommissionsContext commissionsContext;

    [SerializeField, Tooltip("Number of commissions we can select from per 'round'.")]
    private int commissionSetSize = 3;

    /// <summary>
    /// The shared reference to the submit inventory where player will put an item into.
    /// </summary>
    [SerializeField, Required]
    private SO_Inventory submitInventory;
    
    /// <summary>
    /// History of completed commissions and related items to look back on.
    /// </summary>
    private List<Tuple<Commission, Item>> completedCommissions = new List<Tuple<Commission, Item>>();

    private CommissionGenerator commissionGenerator;
    private CommissionSubmissionValidator validator;

    private void Awake()
    {
        Assert.IsNotNull(submitInventory, "Submit Inventory is not set in the inspector.");
        commissionGenerator = new CommissionGenerator(1, 1);
        validator = new CommissionSubmissionValidator();
    }

    private void Start()
    {
        // Reset everything and generate commissions.
        commissionsContext.ResetContext();
        FillCommissions();
    }

    /// <summary>
    /// Helper to fill the commission context to the max pending commissions field.
    /// </summary>
    public void FillCommissions()
    {
        List<Commission> commissions = commissionGenerator.GenerateCommissions(commissionSetSize);

        commissions.ForEach((commission) =>
        {
            commission.OnCommissionStart += AcceptCommission;
            commission.OnCommissionSubmitted += SubmitCommission;
        });

        commissionsContext.Commissions = commissions; // Triggers OnCommissionContextChange event.
    }

    #region Private Helpers Subscribed to Commission Events
    private void AcceptCommission(Commission commission)
    {
        // Redundent Check if the commission is in the list and also is
        // in pending status
        if (!commissionsContext.Commissions.Contains(commission) || commission.commissionStatus != CommissionStatus.PENDING)
        {
            Debug.LogError("Commission not found in list or not in pending status.");
            return;
        }

        // Check if we have already accepted a quest
        if (commissionsContext.ActiveCommission != null)
        {
            Debug.Log("Max active commissions reached.");
            return;
        }

        commission.commissionStatus = CommissionStatus.ACTIVE;
        commissionsContext.ActiveCommission = commission; // Triggers OnCommissionContextChange event.
    }

    /// <summary>
    /// Validates the commission submission alongside the submitted item and then takes the appropriate action.
    /// </summary>
    /// <param name="commission"></param>
    private void SubmitCommission(Commission commission, Item submittedItem)
    {
        if (!validator.ValidateSubmission(commission, submittedItem))
        {
            // CompleteCommission(commission);
            Debug.Log("Submitted item does not meet the commission requirements.");
            return;
        }

        // Check if the submitted commission is the active one.
        if (!commissionsContext.ActiveCommission.Equals(commission))
        {
            Debug.Log("Commission completed is not he active one!");
            return;
        }
        
        HandleCommissionCompletion(commission, submittedItem);
    }

    /// <summary>
    /// Handle Commission Completion. At this point we have validated that the submitted Item is valid!
    /// 
    /// (Note: not in this order lmao)
    /// 
    /// 1. Resets commission context and refills it.
    /// 2. Gives player reward.
    /// 3. Clear submitted item inventory.
    /// 4. Adds to log
    /// 
    /// </summary>
    /// <param name="commission"></param>
    /// <param name="submittedItem"></param>
    private void HandleCommissionCompletion(Commission commission, Item submittedItem)
    {
        // Valid Submission. Move the item to completed commissions list.
        // Add money to Player's wallet.
        completedCommissions.Add(new Tuple<Commission, Item>(commission, submittedItem));
        submitInventory.ClearInventory();

        PlayerSingleton.Instance.GetComponentInChildren<StatComponent>().gold += commission.reward;

        // Reset Active and generate new commission set
        commissionsContext.ResetContext();
        FillCommissions();


        commission.commissionStatus = CommissionStatus.COMPLETED;
        commission.CompleteCommission();
    }
    #endregion
}
