using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.UI;

public class ItemDescView : MonoBehaviour, IView<ItemDescViewConfig>
{
    public Canvas DisplayCanvas => itemDescCanvas;
    /// <summary>
    /// For hiding/showing view.
    /// </summary>
    [Required, SerializeField] private Canvas itemDescCanvas;

    [Required, SerializeField] private PageUI page;

    [FoldoutGroup("Empty View")]
    [Required, SerializeField] private Transform emptyView;

    [FoldoutGroup("Filled View")]
    [Required, SerializeField] private Transform filledView;

    [FoldoutGroup("Filled View")]
    [Required, SerializeField] private TextMeshProUGUI TitleText;

    #region Colours
    [SerializeField] private Color green = new Color32(61, 119, 72, 255);
    [SerializeField] private Color red = new Color32(168, 65, 52, 255);
    [SerializeField] private Color gold = new Color32(151, 103, 9, 255);
    [SerializeField] private Color gray = new Color32(99, 96, 90, 255);
    #endregion

    #region Core Display
    [FoldoutGroup("Filled View/Core Display")]
    [Required, SerializeField] private TextMeshProUGUI DamageText;
    private const string BASE_DAMAGE_FIELD_HEADER_TEXT = "Damage: ";

    [FoldoutGroup("Filled View/Core Display")]
    [Required, SerializeField] private TextMeshProUGUI NumProjectilesText;
    private const string NUM_PROJECTILE_HEADER_TEXT = "Number of Projectiles: ";

    [FoldoutGroup("Filled View/Core Display")]
    [Required, SerializeField] private TextMeshProUGUI ProjectileRangeText;
    private const string PROJECTILE_RANGE_HEADER_TEXT = "Range: ";

    [FoldoutGroup("Filled View/Core Display")]
    [Required, SerializeField] private TextMeshProUGUI ProjectileSpeedText;
    private const string PROJECTILE_SPEED_HEADER_TEXT = "Projectile Speed: ";
    #endregion

    #region Bonus Stats
    // Bonus Stat Fields
    [FoldoutGroup("Filled View/Bonus Stats")]
    [SerializeField] private GameObject BonusStatParentBox;
    [FoldoutGroup("Filled View/Bonus Stats")]
    [SerializeField] private GameObject BonusStatsPfbParent;
    [FoldoutGroup("Filled View/Bonus Stats")]
    [SerializeField] private TextMeshProUGUI BonusStatTextPfb;
    #endregion

    #region Other Stats
    // Trait Fields
    [FoldoutGroup("Filled View/Other Stats")]
    [SerializeField] private GameObject TraitParentBox;
    [FoldoutGroup("Filled View/Other Stats")]
    [SerializeField] private GameObject TraitPfbParent;
    [FoldoutGroup("Filled View/Other Stats")]
    [SerializeField] private TextMeshProUGUI TraitTextPfb;
    #endregion

    #region Visual Displays
    [FoldoutGroup("Filled View/Visual Displays")]
    [SerializeField, Required] private Image weaponImage;

    [FoldoutGroup("Filled View/Visual Displays")]
    [SerializeField, Required] private Image projectileImage;
    #endregion

    /// <summary>
    /// Populates the descriptor view with model data.
    ///
    /// If model is null, shows the empty view (which just has a "No Item Selected" text).
    /// Otherwise, shows the filled view with all relevant data populated.
    /// </summary>
    public void DisplayView(ItemDescViewConfig model)
    {
        itemDescCanvas.enabled = true;
        page.MoveToTop();

        if (model == null)
        {
            ShowEmptyView();
            return;
        } else
        {
            ShowFilledView(model);
        }
    }

    private void ShowEmptyView()
    {
        filledView.gameObject.SetActive(false);
        emptyView.gameObject.SetActive(true);
    }

    private void ShowFilledView (ItemDescViewConfig model)
    {
        // TODO: modify ItemDescViewConfig.
        TitleText.text = model.Name;

        // --- Core Display Stats ---

        // Damage: 50 (30 + 10 + 10)
        DamageText.text = $"<color={ColorTag(gold)}>{BASE_DAMAGE_FIELD_HEADER_TEXT}{model.Damage}</color>" +
            $"<color={ColorTag(gray)}> ({model.BaseAtk} + </color>" +
            $"<color={ColorTag(green)}>{model.BonusAtk}</color>" +
            $"<color={ColorTag(gray)}> + </color>" +
            $"<color={ColorTag(red)}>{model.DamageIncreaseFromPercent}</color>" +
            $"<color={ColorTag(gray)}>)</color>";
       
        // Number of Projectiles: 2 (1 + 1)
        NumProjectilesText.text =
            $"{NUM_PROJECTILE_HEADER_TEXT}" +
            $"<color={ColorTag(gold)}>{model.NumProjectilesPerAttack}</color>";
       
        // Projectile Range: 12 (6 + 6)
        ProjectileRangeText.text =
            $"{PROJECTILE_RANGE_HEADER_TEXT}" +
            $"<color={ColorTag(gold)}>{model.ProjectileRange}</color>";

        // Projectile Speed: 12 (6 + 6)
        ProjectileSpeedText.text =
            $"{PROJECTILE_SPEED_HEADER_TEXT}" +
            $"<color={ColorTag(gold)}>{model.ProjectileSpeed}</color>";


        // --- Bonus Stats ---
        ClearChildren(BonusStatsPfbParent);
        if (model.BonusStats != null && model.BonusStats.Count > 0)
        {
            BonusStatParentBox.SetActive(true);
            foreach (var bonusStat in model.BonusStats)
            {
                var bonusStatText = Instantiate(BonusStatTextPfb, BonusStatsPfbParent.transform);
                bonusStatText.text = $"<color={ColorTag(green)}>{bonusStat.stat}: {bonusStat.value}</color>";
            }

        }
        else
        {
            BonusStatParentBox.SetActive(false);
        }

        // --- Other/Traits ---
        ClearChildren(TraitPfbParent);
        if (model.Traits != null && model.Traits.Count > 0)
        {
            TraitParentBox.SetActive(true);
            foreach (string trait in model.Traits)
            {
                var traitText = Instantiate(TraitTextPfb, TraitPfbParent.transform);
                traitText.text = $"<color={ColorTag(red)}>{trait}</color>";
            }
        }
        else
        {
            TraitParentBox.SetActive(false);
        }

        weaponImage.sprite = model.weaponImage;
        // Rotate weapon image by specified amount (if any).
        weaponImage.transform.rotation = Quaternion.Euler(0, 0, model.weaponImageRot);

        projectileImage.sprite = model.projectileImage;

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

    private string ColorTag(Color color)
    {
        return $"#{ColorUtility.ToHtmlStringRGB(color)}";
    }

    // Not really used, but here to satisfy the IView interface. DisplayView is the main method that should be used to populate and show the view.
    // TODO-OPT: refactor such that we have this supported, but for now just leave it as it is..
    public void ShowView()
    {
        throw new System.NotImplementedException();
    }

    public void HideView()
    {
        throw new System.NotImplementedException();
    }

    public void UpdateView(ItemDescViewConfig config)
    {
        throw new System.NotImplementedException();
    }
}
