using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Component that listens for movement input events and keeps track of the current and last direction of movement.
/// </summary>
public class MotionComponent : MonoBehaviour
{
    public TrackedEvent OnMotionChanged = new TrackedEvent("MotionComponent.OnMotionChanged", false);

    [Required, SerializeField] private LocalEventHandler leh;
    private LocalEventBinding<OnMovementInput> movementInputEventBinding;

    public Vector2 Dir { get; private set; }

    // Last Direction of one of: (1,0), (1, 1), (1, -1), (0, 1), (0, -1), (-1, -1), (-1, 1)
    public Vector2 LastDir { get; private set; }

    private bool wasDiaganol = false;

    private void Awake()
    {
        Dir = Vector2.zero;

        leh = InitializerUtil.FindComponentInParent<LocalEventHandler>(gameObject);
        Assert.IsNotNull(leh);

        movementInputEventBinding = new LocalEventBinding<OnMovementInput>(OnMovementInput);
    }

    private void OnEnable()
    {
        leh.Register<OnMovementInput>(movementInputEventBinding);
    }

    private void OnDisable()
    {
        leh.Unregister<OnMovementInput>(movementInputEventBinding);
    }


    private void OnMovementInput(OnMovementInput input)
    {
        float offset = 0.1f;
        this.Dir = input.movementInput;

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
        } else
        {
            wasDiaganol = false;
        }

        if (Math.Abs(r_x) == 1 || Math.Abs(r_y) == 1)
        {
            LastDir = new Vector2(r_x, r_y);
        }

        leh.Call(new OnMotionChangeEvent { newDir = Dir, lastDir = LastDir });
        OnMotionChanged?.Invoke();
    }
}