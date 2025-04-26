using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For tracking the player between scenes.
public class CameraPlayerTracker : MonoBehaviour
{
    [Tooltip("The virtual camera to track the player with.")]
    [SerializeField] private Cinemachine.CinemachineVirtualCamera _virtualCamera;

    private void Awake()
    {
        // If there is no virtual camera, then find one.
        if (_virtualCamera == null)
        {
            _virtualCamera = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
        }
        
        // If there is still no virtual camera, then log an error.
        if (_virtualCamera == null)
        {
            Debug.LogError("No virtual camera found.");
        }
    }

    private void Start()
    {
        // If there is a player, then track the player.
        if (PlayerSingleton.Instance != null)
        {
            _virtualCamera.Follow = PlayerSingleton.Instance.transform;
            _virtualCamera.LookAt = PlayerSingleton.Instance.transform;
        }
    }
}
