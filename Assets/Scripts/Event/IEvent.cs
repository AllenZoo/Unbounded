using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public interface IEvent
{

}

/// <summary>
/// For events that act globally. (Global Bus that can connect multiple entities)
/// </summary>
public interface IGlobalEvent: IEvent
{

}

public struct OnMapGeneratedEvent : IGlobalEvent
{
    public GameObject startRoomPfb;
}

public struct OnPlayerEnterRoom : IGlobalEvent
{
    public GameObject roomPfb;
    public Collider2D roomBoundary;
}

public struct OnPlayerStayRoom: IGlobalEvent
{
    public GameObject roomPfb;
    public Collider2D roomBoundary;
}

public struct OnPlayerExitRoom: IGlobalEvent
{
    public GameObject roomPfb;
    public Collider2D roomBoundary;
}

public struct OnInventoryModifiedEvent : IGlobalEvent
{
    
}

public struct OnCommissionViewInfoRequestEvent: IGlobalEvent
{
    public Commission commission;
}

public struct OnSceneLoadRequest: IGlobalEvent
{
    public List<SceneField> scenesToLoad;
    public List<SceneField> scenesToUnload;
    public SceneField activeSceneToSet;
    public bool showLoadingBar;
}

public struct OnSceneLoadRequestFinish: IGlobalEvent
{

}

public struct OnCameraBoundChangeRequest: IGlobalEvent
{
    public Collider2D newBoundary;
}

public struct OnPauseChangeRequest: IGlobalEvent {
    public bool shouldPause;
}

public struct OnUpgradeCardApplyEffect: IGlobalEvent
{
    public UpgradeCardData cardData;
}
public struct OnDisplayUpgradeCardsRequest: IGlobalEvent
{
    public HashSet<UpgradeCardData> upgradeCards;
}

public struct OnStarterWeaponCardApplyEffect : IGlobalEvent
{
    public StarterWeaponData cardData;
}
public struct OnDisplayStaterWeaponCardsRequest : IGlobalEvent
{
    public HashSet<StarterWeaponData> starterWeaponCards;
}

// Invoked in editor via EventInvoker.cs
public struct OnTutorialObjectiveRequest : IGlobalEvent
{

}

/// <summary>
/// For events that act locally. (Personal Buses for any entity)
/// </summary>
public interface ILocalEvent : IEvent { }

public struct OnMotionChangeEvent: ILocalEvent
{
    public Vector2 lastDir;
    public Vector2 newDir;
}

public struct OnMovementInput: ILocalEvent
{
    public Vector2 movementInput;
}

public struct OnAttackInput: ILocalEvent
{
    public KeyCode keyCode;
    public AttackSpawnInfo attackInfo;
}

public struct OnDamagedEvent: ILocalEvent
{
    public float damage;
}

public struct OnDeathEvent : ILocalEvent { }

public struct OnStateChangeEvent: ILocalEvent {
    public State newState;
    public State oldState;
}

public struct OnKnockBackBeginEvent: ILocalEvent
{
    public Vector2 knockbackDir;
    public float knockbackForce;
}

public struct OnKnockBackEndEvent: ILocalEvent { }

public struct OnStatChangeEvent: ILocalEvent
{
    public StatComponent statComponent;
}

public struct OnSpawnEvent: ILocalEvent
{
    public Spawnable spawn;
}

public struct OnDespawnEvent: ILocalEvent
{
    public Spawnable spawn;
}

public struct OnAggroStatusChangeEvent: ILocalEvent
{
    public bool isAggroed;
}

public struct OnWeaponEquippedEvent : ILocalEvent
{
    public Item equipped;
    public Item unequipped;
}

public struct OnStatBuffEvent: ILocalEvent
{
    public StatModifier buff;
}