using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MonoBehaviour class attached to UpgradeCardPack pfb.
/// </summary>
public class UpgradeCardPack : CardPackBase<UpgradeCardData, OnDisplayUpgradeCardsRequest>
{
    protected override OnDisplayUpgradeCardsRequest CreateDisplayEvent(HashSet<UpgradeCardData> cards)
    {
        return new OnDisplayUpgradeCardsRequest { upgradeCards = cards };
    }
}
