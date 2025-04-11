using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBoundarySetter : MonoBehaviour
{
    [SerializeField] private Collider2D boundaryToSet;
    [SerializeField] private bool setOnEnable = true;

    private void Start()
    {
        RequestCameraBoundChange();
    }

    private void OnEnable()
    {
        if (setOnEnable)
        {
            RequestCameraBoundChange();
        }
    }

    private void RequestCameraBoundChange()
    {
        Debug.Log("Requesting Camera Bound Change!");
        EventBus<OnCameraBoundChangeRequest>.Call(new OnCameraBoundChangeRequest { newBoundary = boundaryToSet });
    }
}
