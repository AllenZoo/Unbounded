using UnityEngine;

/// <summary>
/// Script to handle OnClick, hover events, etc. on the Starter Weapon Card.
/// Should only handle logic of passing Starter Weapon Card to any listeners for these events.
/// Hover animations should be hanlded by MenuEventSystemHandler.cs
/// </summary>
[RequireComponent(typeof(StarterWeaponCardView))]
public class StarterWeaponCardViewEventHandler : CardViewEventHandlerBase<StarterWeaponCardView>
{

}