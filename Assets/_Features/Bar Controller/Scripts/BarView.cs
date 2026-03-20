using Sirenix.OdinInspector;
using TMPro;
using Unity.AppUI.Core;
using UnityEngine;
using UnityEngine.UI;

public class BarView : MonoBehaviour, IView<BarConfig>
{
    // For hiding and showing display.
    [Required, SerializeField] private Canvas display;
    [Required, SerializeField] private Image fillImage;

    // Text Component is required, but displaying it is not.
    [Required, SerializeField] private TextMeshProUGUI text;

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
        throw new System.NotImplementedException();
        //if (!this) return;
        //if (!isActiveAndEnabled) return;
        //if (!fillImage) return;

        //if (useBarContext)
        //{
        //    displayUI?.SetActive(barContext.IsVisible);
        //}

        //if (statObject == null || statObject.StatContainer == null) return;

        //float fill = 0f;

        //switch (statToTrack)
        //{
        //    case BarTrackStat.HP:
        //        float max = statObject.StatContainer.MaxHealth;
        //        fill = max > 0 ? statObject.StatContainer.Health / max : 0f;
        //        break;
        //}
        //fillImage.fillAmount = Mathf.Clamp01(fill);
    }
}
