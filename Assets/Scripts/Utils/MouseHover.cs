using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script intended for objects that need to follow mouse.
// Attach to objects that follow mouse.
public class MouseHover : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
   
    public void Awake()
    {
        canvas = transform.root.GetComponent<Canvas>();
    }

    private void Update()
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

    private void OnDisable()
    {
        // Move to bottom left corner of screen.
        transform.position = new Vector3(-1000, -1000, 0);
    }

    public void Toggle(bool turnOn)
    {
        gameObject.SetActive(turnOn);
    }

   

}
