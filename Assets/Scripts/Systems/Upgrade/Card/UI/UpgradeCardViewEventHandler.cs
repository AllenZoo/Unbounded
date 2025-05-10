using UnityEngine;

/// <summary>
/// Script to handle OnClick, hover events, etc. on the Upgrade Card.
/// Should only handle logic of passing Upgrade Card to any listeners for these events.
/// Hover animations should be hanlded by MenuEventSystemHandler.cs
/// </summary>
[RequireComponent(typeof(UpgradeCardView))]
public class UpgradeCardViewEventHandler : CardViewEventHandlerBase<UpgradeCardView>
{

}

