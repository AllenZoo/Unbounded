using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageUI : MonoBehaviour, IUIPage
{
    [Required]
    [SerializeField] private Canvas canvas;



    private void Awake()
    {
        if (canvas == null)
        {
            canvas = GetComponent<Canvas>();
        }

        if (canvas == null)
        {
            Debug.LogError("Canvas not found in PageUI.");
        }
    }

    private void OnEnable()
    {
        // UIOverlayManager.Instance.BringToFront(this);
    }

    private void Start()
    {
        // UIOverlayManager.Instance.AddUIPage(this);
    }

    public Canvas GetCanvas()
    {
        return canvas;
    }
}
