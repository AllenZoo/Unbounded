using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoverColour : MonoBehaviour
{
    [SerializeField] private Button hoverButton;
    [SerializeField] private Color hoverColour;
    [SerializeField] private Color defaultSelectColour;
    private ColorBlock cb;

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
}
