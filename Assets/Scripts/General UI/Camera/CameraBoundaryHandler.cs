using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CameraBoundaryHandler : MonoBehaviour
{
    [SerializeField] private CinemachineConfiner2D confiner;
    // [SerializeField] private CinemachineVirtualCamera mainVirtualCam;

    private void Awake()
    {
        Assert.IsNotNull(confiner, "No CinemachineConfiner2D assigned to CameraBoundaryHandler.");

        EventBinding<OnCameraBoundChangeRequest> boundChangeBinding = new EventBinding<OnCameraBoundChangeRequest>(OnCameraBoundChangeRequestEvent);
        EventBus<OnCameraBoundChangeRequest>.Register(boundChangeBinding);
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
        //confiner.m_BoundingShape2D = e.roomBoundary;
    }

    private void HandlePlayerStayRoom(OnPlayerStayRoom e)
    {
        //confiner.m_BoundingShape2D = e.roomBoundary;
    }

    private void OnCameraBoundChangeRequestEvent(OnCameraBoundChangeRequest e)
    {
        Debug.Log("Set new camera boundary!");
        confiner.m_BoundingShape2D = e.newBoundary;
    }
}
