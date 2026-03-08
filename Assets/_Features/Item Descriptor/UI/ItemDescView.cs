using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class ItemDescView : MonoBehaviour
{
    /// <summary>
    /// For hiding/showing view.
    /// </summary>
    [Required, SerializeField] private Canvas itemDescCanvas;

    [FoldoutGroup("Empty View")]
    [Required, SerializeField] private Transform emptyView;

    [FoldoutGroup("Filled View")]
    [Required, SerializeField] private Transform filledView;

    [FoldoutGroup("Filled View")]
    [SerializeField] private TextMeshProUGUI TitleText;

    [FoldoutGroup("Filled View")]
    [SerializeField] private TextMeshProUGUI BaseDamageText;
    private const string BASE_DAMAGE_FIELD_HEADER_TEXT = "Base Damage: ";

    [FoldoutGroup("Filled View")]
    [SerializeField] private TextMeshProUGUI FinalDamageText;
    private const string FINAL_DAMAGE_FIELD_HEADER_TEXT = "Final Damage: ";

    [FoldoutGroup("Filled View")]
    [SerializeField] private TextMeshProUGUI ProjectileSpeedText;
    private const string PROJECTILE_SPEED_HEADER_TEXT = "Projectile Speed: ";

    [FoldoutGroup("Filled View")]
    [SerializeField] private TextMeshProUGUI ProjectileRangeText;
    private const string PROJECTILE_RANGE_HEADER_TEXT = "Projectile Range: ";

    [FoldoutGroup("Filled View")]
    [SerializeField] private TextMeshProUGUI NumProjectilesText;
    private const string NUM_PROJECTILE_HEADER_TEXT = "Number of Projectiles: ";

    // Bonus Stat Fields
    [FoldoutGroup("Filled View")]
    [SerializeField] private GameObject BonusStatParentBox;
    [FoldoutGroup("Filled View")]
    [SerializeField] private GameObject BonusStatsPfbParent;
    [FoldoutGroup("Filled View")]
    [SerializeField] private TextMeshProUGUI BonusStatTextPfb;

    // Trait Fields
    [FoldoutGroup("Filled View")]
    [SerializeField] private GameObject TraitParentBox;
    [FoldoutGroup("Filled View")]
    [SerializeField] private GameObject TraitPfbParent;
    [FoldoutGroup("Filled View")]
    [SerializeField] private TextMeshProUGUI TraitTextPfb;

    /// <summary>
    /// Populates the descriptor view with model data.
    /// 
    /// If model is null, shows the empty view (which just has a "No Item Selected" text).
    /// Otherwise, shows the filled view with all relevant data populated.
    /// </summary>
    public void DisplayView(ItemDescViewConfig model)
    {
        itemDescCanvas.enabled = true;

        if (model == null)
        {
            ShowEmptyView();
            return;
        } else
        {
            ShowFilledView(model);
        }
    }

    /// <summary>
    /// Hides the descriptor view.
    /// </summary>
    public void HideView()
    {
        itemDescCanvas.enabled = false;

        // Optional cleanup (so stale data isn’t shown when redisplayed)
        ClearChildren(BonusStatsPfbParent);
        ClearChildren(TraitPfbParent);
    }

    private void ShowEmptyView()
    {
        filledView.gameObject.SetActive(false);
        emptyView.gameObject.SetActive(true);
    }

    private void ShowFilledView (ItemDescViewConfig model)
    {
        TitleText.text = model.Name;

        // --- Core Stats ---
        BaseDamageText.text = $"{BASE_DAMAGE_FIELD_HEADER_TEXT}{model.BaseAtk}";
        FinalDamageText.text = $"{FINAL_DAMAGE_FIELD_HEADER_TEXT}{model.FinalAtk}";
        ProjectileSpeedText.text = $"{PROJECTILE_SPEED_HEADER_TEXT}{model.ProjectileSpeed}";
        ProjectileRangeText.text = $"{PROJECTILE_RANGE_HEADER_TEXT}{model.ProjectileRange}";
        NumProjectilesText.text = $"{NUM_PROJECTILE_HEADER_TEXT}{model.NumProjectilesPerAttack}";

        // --- Bonus Stats ---
        ClearChildren(BonusStatsPfbParent);
        if (model.BonusStats != null && model.BonusStats.Count > 0)
        {
            BonusStatParentBox.SetActive(true);
            foreach (var bonusStat in model.BonusStats)
            {
                var bonusStatText = Instantiate(BonusStatTextPfb, BonusStatsPfbParent.transform);
                bonusStatText.text = $"{bonusStat.stat}: {bonusStat.value}";
            }

        }
        else
        {
            BonusStatParentBox.SetActive(false);
        }

        // --- Traits ---
        ClearChildren(TraitPfbParent);
        if (model.Traits != null && model.Traits.Count > 0)
        {
            TraitParentBox.SetActive(true);
            foreach (string trait in model.Traits)
            {
                var traitText = Instantiate(TraitTextPfb, TraitPfbParent.transform);
                traitText.text = trait;
            }
        }
        else
        {
            TraitParentBox.SetActive(false);
        }

        filledView.gameObject.SetActive(true);
        emptyView.gameObject.SetActive(false);
    }

    /// <summary>
    /// Utility to remove all instantiated children under a parent.
    /// </summary>
    private void ClearChildren(GameObject parent)
    {
        foreach (Transform child in parent.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
