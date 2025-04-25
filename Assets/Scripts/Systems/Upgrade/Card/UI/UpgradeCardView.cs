using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCardView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Transform modifierListParent;
    [SerializeField] private ModifierView modifierViewPrefab;

    public void Render(UpgradeCardData data)
    {
        if (data == null)
        {
            Debug.LogError("Tried rendering null upgrade card data!");
            return;
        }

        titleText.text = data.title;
        iconImage.sprite = data.icon;
        backgroundImage.color = data.cardColor;

        foreach (Transform child in modifierListParent)
            Destroy(child.gameObject);

        foreach (var entry in data.mods)
        {
            var modView = Instantiate(modifierViewPrefab, modifierListParent);
            modView.SetModifier(entry.modifier, entry.modifierDescription);
        }
    }
}

