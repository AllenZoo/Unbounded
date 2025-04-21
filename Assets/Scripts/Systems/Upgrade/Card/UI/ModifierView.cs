using UnityEngine;
using TMPro;

public class ModifierView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI descriptionText;

    public void SetModifier(IUpgradeModifier modifier, string description)
    {
        // You could switch based on modifier type or use polymorphism
        descriptionText.text = GetFriendlyText(modifier, description);
    }

    private string GetFriendlyText(IUpgradeModifier modifier, string fallback)
    {
        if (modifier is StatModifier statMod)
        {
            return $"Modifies {statMod.Stat} by {statMod.operation.GetValue()}";
        }
        if (modifier is DamageModifier)
        {
            return "Deals bonus damage!";
        }
        if (modifier is TraitModifier)
        {
            return "Grants a trait!";
        }

        return fallback;
    }
}
