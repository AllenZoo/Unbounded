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

    [SerializeField] private TextMeshProUGUI TitleText;
    
    [SerializeField] private TextMeshProUGUI BaseDamageText;
    private const string BASE_DAMAGE_FIELD_HEADER_TEXT = "Base Damage: ";

    [SerializeField] private TextMeshProUGUI FinalDamageText;
    private const string FINAL_DAMAGE_FIELD_HEADER_TEXT = "Final Damage: ";

    [SerializeField] private TextMeshProUGUI ProjectileSpeedText;
    private const string PROJECTILE_SPEED_HEADER_TEXT = "Projectile Speed: ";

    [SerializeField] private TextMeshProUGUI ProjectileRangeText;
    private const string PROJECTILE_RANGE_HEADER_TEXT = "Projectile Range: ";

    [SerializeField] private TextMeshProUGUI NumProjectilesText;
    private const string NUM_PROJECTILE_HEADER_TEXT = "Number of Projectiles: ";

    // Bonus Stat Fields
    [SerializeField] private GameObject BonusStatParentBox;
    [SerializeField] private GameObject BonusStatsPfbParent;
    [SerializeField] private TextMeshProUGUI BonusStatTextPfb;

    // Trait Fields
    [SerializeField] private GameObject TraitParentBox;
    [SerializeField] private GameObject TraitPfbParent;
    [SerializeField] private TextMeshProUGUI TraitTextPfb;

    /// <summary>
    /// Populates the descriptor view with model data.
    /// </summary>
    public void DisplayView(ItemDescModel model)
    {
        itemDescCanvas.enabled = true;

        TitleText.text = model.Name;

        // --- Core Stats ---
        BaseDamageText.text = $"{BASE_DAMAGE_FIELD_HEADER_TEXT}{model.BaseAtk}";
        FinalDamageText.text = $"{FINAL_DAMAGE_FIELD_HEADER_TEXT}{model.FinalAtk}";
        ProjectileSpeedText.text = $"{PROJECTILE_SPEED_HEADER_TEXT}{model.ProjectileSpeed}";
        ProjectileRangeText.text = $"{PROJECTILE_RANGE_HEADER_TEXT}{model.ProjectileRange}";
        NumProjectilesText.text = $"{NUM_PROJECTILE_HEADER_TEXT}{model.NumProjectilesPerAttack}";

        // --- Bonus Stats ---
        ClearChildren(BonusStatsPfbParent);
        if (model.BonusStats != null && model.BonusStats.Count > 0) {
            BonusStatParentBox.SetActive(true);
            foreach (var bonusStat in model.BonusStats)
            {
                var bonusStatText = Instantiate(BonusStatTextPfb, BonusStatsPfbParent.transform);
                bonusStatText.text = $"{bonusStat.stat}: {bonusStat.value}";
            }

        } else
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
        } else
        {
            TraitParentBox.SetActive(false);
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
