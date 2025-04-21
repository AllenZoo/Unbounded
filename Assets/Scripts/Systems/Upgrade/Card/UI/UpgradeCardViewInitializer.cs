using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeCardViewInitializer : MonoBehaviour
{
    [SerializeField] private UpgradeCardData upgradeCardData;
    [SerializeField] private UpgradeCardView cardView;

    private void Start()
    {
        if (upgradeCardData != null && cardView != null)
        {
            cardView.Render(upgradeCardData);
        }
        else
        {
            Debug.LogWarning("Missing data or view reference on UpgradeCardInitializer.");
        }
    }

    public void SetData(UpgradeCardData data)
    {
        upgradeCardData = data;
        cardView.Render(data);
    }
}
