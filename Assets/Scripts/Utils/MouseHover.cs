using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script intended for objects that need to follow mouse.
public class MouseHover : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
   
    public void Awake()
    {
        canvas = transform.root.GetComponent<Canvas>();
    }

    public void Update()
    {
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)canvas.transform,
            Input.mousePosition,
            canvas.worldCamera,
            out position
                );
        transform.position = canvas.transform.TransformPoint(position);
    }

    public void Toggle(bool turnOn)
    {
        gameObject.SetActive(turnOn);
    }

}
