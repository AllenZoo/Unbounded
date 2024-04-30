using System.Collections;
using System.Collections.Generic;
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

/// <summary>
/// For events that act locally. (Personal Buses for any entity)
/// </summary>
public interface  ILocalEvent: IEvent
{
    
}

public struct OnDamagedEvent: ILocalEvent
{
    public float damage;
}
