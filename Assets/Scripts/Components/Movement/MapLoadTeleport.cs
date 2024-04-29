using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles teleporting an entity to middle of start room when map is generated.
/// </summary>
public class MapLoadTeleport : MonoBehaviour
{
    [Tooltip("Entity to teleport to start room")]
    [SerializeField] private GameObject entity;

    private IEventBinding<OnMapGeneratedEvent> eventBinding;

    private void Awake()
    {
        if (entity == null)
        {
            Debug.LogWarning("Entity not set/serialized for MapLoadTeleport.cs");
        }

        eventBinding = new EventBinding<OnMapGeneratedEvent>(TeleportEntity);
    }

    private void Start()
    {
        EventBus<OnMapGeneratedEvent>.Register(eventBinding);
    }

    private void TeleportEntity(OnMapGeneratedEvent @event)
    {
        if (entity != null)
        {
            entity.transform.position = @event.startRoomPfb.transform.position;
        }
    }
}
