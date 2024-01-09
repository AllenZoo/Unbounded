using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MotionComponent : MonoBehaviour
{
    [SerializeField] private InputController inputController;

    // Dir of motion. Last dir of motion.
    public event Action<Vector2, Vector2> OnMotionChange;
    public Vector2 dir { get; private set; }

    // Last Direction of one of: (1,0), (1, 1), (1, -1), (0, 1), (0, -1), (-1, -1), (-1, 1)
    public Vector2 lastDir { get; private set; }

    private bool wasDiaganol = false;

    private void Awake()
    {
        if (inputController == null)
        {
            inputController = GetComponent<InputController>();
        }
        
        Assert.IsNotNull(inputController, "Motion Component requires a reference to an inputController.");
        dir = Vector2.zero;
    }

    private void Start()
    {
        inputController.OnMovementInput += OnMovementInput;
    }

    private void OnMovementInput(Vector2 dir)
    {
        float offset = 0.1f;
        this.dir = dir;

        float r_x = Mathf.Round(dir.x);
        float r_y = Mathf.Round(dir.y);

        float abs_x = Mathf.Abs(dir.x);
        float abs_y = Mathf.Abs(dir.y);


        if (dir.x == 0 && dir.y == 0)
        {
            wasDiaganol = false;
        }

        if (wasDiaganol && Mathf.Abs(abs_x - abs_y) < offset)
        {
            return;
        }

        if (Math.Abs(r_x) == 1 && Math.Abs(r_y) == 1)
        {
            wasDiaganol = true;
        } else
        {
            wasDiaganol = false;
        }

        if (Math.Abs(r_x) == 1 || Math.Abs(r_y) == 1)
        {
            lastDir = new Vector2(r_x, r_y);
        }

        OnMotionChange?.Invoke(dir, lastDir);
    }
}
