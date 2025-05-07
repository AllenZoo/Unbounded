using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaterWeaponCardPack : CardPackBase<StarterWeaponData, OnDisplayStaterWeaponCardsRequest>
{
    protected override OnDisplayStaterWeaponCardsRequest CreateDisplayEvent(HashSet<StarterWeaponData> cards)
    {
        return new OnDisplayStaterWeaponCardsRequest { starterWeaponCards = cards };
    }
}
