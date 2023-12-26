using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverColour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Button hoverButton;
    [SerializeField] private Color hoverColour;
    [SerializeField] private Color defaultSelectColour;
    private ColorBlock cb;

    private void Awake()
    {
        if (hoverButton == null)
        {
            try
            {
                hoverButton = GetComponent<Button>();
            } catch
            {
                Debug.LogError("HoverColour.cs: No button found");
            }
        }
    }

    private void Start()
    {
        cb = hoverButton.colors;
        hoverColour = cb.highlightedColor;
        defaultSelectColour = cb.selectedColor;
    }

    public void Hover()
    {
        cb.selectedColor = hoverColour;
        hoverButton.colors = cb;
    }

    public void Unhover()
    {
        cb.selectedColor = defaultSelectColour;
        hoverButton.colors = cb;
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        Hover();
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        Unhover();
    }
}
