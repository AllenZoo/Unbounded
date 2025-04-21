using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeCardView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Transform modifierListParent;
    [SerializeField] private ModifierView modifierViewPrefab;

    public void SetData(UpgradeCardData data)
    {
        titleText.text = data.title;
        iconImage.sprite = data.icon;
        backgroundImage.color = data.cardColor;

        // Clear old children
        foreach (Transform child in modifierListParent)
            Destroy(child.gameObject);

        // Instantiate modifier views
        foreach (var entry in data.mods)
        {
            // TODO-OPT: look into object pooling.
            var modView = Instantiate(modifierViewPrefab, modifierListParent);
            modView.SetModifier(entry.modifier, entry.modifierDescription);
        }
    }
}
