using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraBoundaryHandler : MonoBehaviour
{
    [SerializeField] private CinemachineConfiner2D confiner;
    // [SerializeField] private CinemachineVirtualCamera mainVirtualCam;

    private void Awake()
    {
        if (confiner == null)
        {
            Debug.LogError("No CinemachineConfiner2D assigned to CameraBoundaryHandler.");
        }
    }

    private void Start()
    {
        EventBinding<OnPlayerEnterRoom> playerEnterRoomBinding = new EventBinding<OnPlayerEnterRoom>(HandlePlayerEnterRoom);
        EventBus<OnPlayerEnterRoom>.Register(playerEnterRoomBinding);

        EventBinding<OnPlayerStayRoom> playerStayBinding = new EventBinding<OnPlayerStayRoom>(HandlePlayerStayRoom);
        EventBus<OnPlayerStayRoom>.Register(playerStayBinding);
    }

    private void HandlePlayerEnterRoom(OnPlayerEnterRoom e)
    {
        confiner.m_BoundingShape2D = e.roomBoundary;
    }

    private void HandlePlayerStayRoom(OnPlayerStayRoom e)
    {
        confiner.m_BoundingShape2D = e.roomBoundary;
    }


}
