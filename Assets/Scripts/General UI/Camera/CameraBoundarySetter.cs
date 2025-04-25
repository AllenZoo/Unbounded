using UnityEngine;
using UnityEngine.Assertions;

public class CameraBoundarySetter : MonoBehaviour
{
    [SerializeField] private Collider2D boundaryToSet;
    [SerializeField] private bool setOnEnable = true;

    private void Awake()
    {
        Assert.IsNotNull(boundaryToSet);
        EventBinding<OnSceneLoadRequestFinish> sceneFinishBinding = new EventBinding<OnSceneLoadRequestFinish>(RequestCameraBoundChange);
        EventBus<OnSceneLoadRequestFinish>.Register(sceneFinishBinding);
    }

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
