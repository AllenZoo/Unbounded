using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BarView : MonoBehaviour, IView<BarConfig>
{
    // For hiding and showing display.
    [Required, SerializeField] private Canvas display;
    [Required, SerializeField] private Image fillImage;

    // Text Component is required, but displaying it is not.
    [Required, SerializeField] private TextMeshProUGUI barOwnerText;

    // Text Component is required, but displaying it is not.
    [Required, SerializeField] private TextMeshProUGUI valueText;

    public Canvas DisplayCanvas => display;

    public void ShowView()
    {
        display.enabled = true;
    }

    public void HideView()
    {
        display.enabled = false;
    }

    public void UpdateView(BarConfig config)
    {
        float fill = config.MaxValue > 0 ? config.CurrentValue / config.MaxValue : 0f;
        valueText.text = config.BarValueText;
        barOwnerText.text = config.BarOwnerText;
        fillImage.fillAmount = Mathf.Clamp01(fill);
    }
}
