using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MonoBehaviour class attached to UpgradeCardPack pfb.
/// </summary>
public class UpgradeCardPack : CardPackBase<UpgradeCardData, OnDisplayUpgradeCardsRequest>
{
    [Tooltip("Whether we disable the pack after player selects an option.")]
    [SerializeField] private bool disableAfterUse = true;


    private EventBinding<OnUpgradeCardApplyEffect> oucaeBinding;

    protected void Start()
    {
        if (disableAfterUse)
        {
            oucaeBinding = new EventBinding<OnUpgradeCardApplyEffect>(HandleApplyUpgradeCard);
            EventBus<OnUpgradeCardApplyEffect>.Register(oucaeBinding);
        }
    }

    protected override OnDisplayUpgradeCardsRequest CreateDisplayEvent(HashSet<UpgradeCardData> cards)
    {
        return new OnDisplayUpgradeCardsRequest { upgradeCards = cards, src = this };
    }

    private void HandleApplyUpgradeCard(OnUpgradeCardApplyEffect e)
    {
        if (disableAfterUse)
        {
            this.gameObject.SetActive(false);
            //EventBus<OnUpgradeCardApplyEffect>.Unregister(oucaeBinding);
        }
    }

    private void OnDisable()
    {
        if (oucaeBinding != null)
        {
            EventBus<OnUpgradeCardApplyEffect>.Unregister(oucaeBinding);
        }
    }
}
