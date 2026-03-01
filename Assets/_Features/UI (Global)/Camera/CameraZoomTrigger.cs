using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


[RequireComponent(typeof(Collision2D))]
public class CameraZoomTrigger : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private float zoomedInSize = 5.0f;
    [SerializeField] private float zoomDuration = 1.0f;
    private float defaultSize;

    private void Start()
    {
        defaultSize = virtualCamera.m_Lens.OrthographicSize;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(ZoomIn());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(ZoomOut());
        }
    }

    private IEnumerator ZoomIn()
    {
        if (virtualCamera != null)
        {
            float elapsedTime = 0f;
            LensSettings lensSettings = virtualCamera.m_Lens;
            
            while (elapsedTime < zoomDuration)
            {
                lensSettings.OrthographicSize = Mathf.Lerp(defaultSize, zoomedInSize, elapsedTime / zoomDuration);
                virtualCamera.m_Lens = lensSettings;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }

    private IEnumerator ZoomOut()
    {
        if (virtualCamera != null)
        {
            float elapsedTime = 0f;
            LensSettings lensSettings = virtualCamera.m_Lens;

            while (elapsedTime < zoomDuration)
            {
                lensSettings.OrthographicSize = Mathf.Lerp(zoomedInSize, defaultSize, elapsedTime / zoomDuration);
                virtualCamera.m_Lens = lensSettings;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }
}

