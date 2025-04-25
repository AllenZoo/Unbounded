using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class MotionComponent : MonoBehaviour
{
    [NotNull]
    [SerializeField] private LocalEventHandler localEventHandler;

    public Vector2 dir { get; private set; }

    // Last Direction of one of: (1,0), (1, 1), (1, -1), (0, 1), (0, -1), (-1, -1), (-1, 1)
    public Vector2 lastDir { get; private set; }

    private bool wasDiaganol = false;

    private void Awake()
    {
        dir = Vector2.zero;

        if (localEventHandler == null)
        {
            localEventHandler = GetComponentInParent<LocalEventHandler>();
            if (localEventHandler == null)
            {
                Debug.LogError("LocalEventHandler unassgined and not found in parent for object [" + gameObject +
                    "] with root object [" + gameObject.transform.root.name + "]");
            }
        }
    }

    private void Start()
    {
        LocalEventBinding<OnMovementInput> eventBinding = new LocalEventBinding<OnMovementInput>(OnMovementInput);
        localEventHandler.Register<OnMovementInput>(eventBinding);
    }

    private void OnMovementInput(OnMovementInput input)
    {
        float offset = 0.1f;
        this.dir = input.movementInput;

        float r_x = Mathf.Round(input.movementInput.x);
        float r_y = Mathf.Round(input.movementInput.y);

        float abs_x = Mathf.Abs(input.movementInput.x);
        float abs_y = Mathf.Abs(input.movementInput.y);


        if (input.movementInput.x == 0 && input.movementInput.y == 0)
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
        }
        else
        {
            wasDiaganol = false;
        }

        if (Math.Abs(r_x) == 1 || Math.Abs(r_y) == 1)
        {
            lastDir = new Vector2(r_x, r_y);
        }

        localEventHandler.Call(new OnMotionChangeEvent { newDir = dir, lastDir = lastDir });
    }
}
