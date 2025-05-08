using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

/// <summary>
/// Handles the creation, display, and interaction logic for upgrade card UI elements.
/// Subscribes to upgrade card events, manages instantiation of card prefabs,
/// and relays user selections to other systems.
/// </summary>
[RequireComponent(typeof(MenuEventSystemHandler))]
public class UpgradeCardViewSystem : CardViewSystemBase<UpgradeCardData, OnDisplayUpgradeCardsRequest, OnUpgradeCardApplyEffect, UpgradeCardView>
{
    protected override OnUpgradeCardApplyEffect CreateApplyEvent(UpgradeCardData cardData)
    {
        return new OnUpgradeCardApplyEffect { cardData = cardData };
    }

    protected override IEnumerable<UpgradeCardData> GetCardListFromEvent(OnDisplayUpgradeCardsRequest e)
    {
        return e.upgradeCards;
    }
}
