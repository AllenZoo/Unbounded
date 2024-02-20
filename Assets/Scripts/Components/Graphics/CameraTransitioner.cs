using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// 
public class CameraTransitioner : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera transitioningCamera;

    [Tooltip("The size of the lens to transition to.")]
    [SerializeField] private float transitionLensSize;

    [SerializeField] private UnityEvent transitionEvent;

    [Tooltip("Optional. The event called to exit transition.")]
    [SerializeField] private UnityEvent transitionCompleteEvent;

    private float oldLensSize;


    private void Start()
    {

        Debug.Assert(transitioningCamera != null, "camera is null. Please assign it.");

        if (transitionEvent == null)
        {
            Debug.LogError("transitionEvent is null. Please assign a UnityEvent to transitionEvent.");
        } else
        {
            transitionEvent.AddListener(Transition);
        }
        

        if (transitionCompleteEvent != null)
        {
           transitionCompleteEvent.AddListener(TransitionComplete);
        }
    }

    private void Transition()
    {
        oldLensSize = transitioningCamera.m_Lens.OrthographicSize;
        ChangeLensSize(transitionLensSize);
    }

    private void TransitionComplete()
    {
        ChangeLensSize(oldLensSize);
    }

    private void ChangeLensSize(float newSize)
    {
        LensSettings lensSettings = transitioningCamera.m_Lens;
        lensSettings.OrthographicSize = newSize;
    }
}
