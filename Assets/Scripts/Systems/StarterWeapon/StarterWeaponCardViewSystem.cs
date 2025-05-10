using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarterWeaponCardViewSystem : CardViewSystemBase<StarterWeaponData, OnDisplayStaterWeaponCardsRequest, OnStarterWeaponCardApplyEffect,  StarterWeaponCardView>
{
    protected override OnStarterWeaponCardApplyEffect CreateApplyEvent(StarterWeaponData cardData)
    {
        return new OnStarterWeaponCardApplyEffect { cardData = cardData };
    }

    protected override IEnumerable<StarterWeaponData> GetCardListFromEvent(OnDisplayStaterWeaponCardsRequest e)
    {
        return e.starterWeaponCards;
    }
}
