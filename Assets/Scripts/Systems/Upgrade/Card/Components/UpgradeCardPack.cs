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

    protected void Start()
    {
        if (disableAfterUse)
        {
            EventBinding<OnUpgradeCardApplyEffect> onUpgradeCardApplyEffectBinding = new EventBinding<OnUpgradeCardApplyEffect>(HandleApplyUpgradeCard);
            EventBus<OnUpgradeCardApplyEffect>.Register(onUpgradeCardApplyEffectBinding);
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
        }
    }
}
