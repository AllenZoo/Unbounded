using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEvent
{

}

public struct OnMapGeneratedEvent: IEvent
{
    public Room startRoom;
}
