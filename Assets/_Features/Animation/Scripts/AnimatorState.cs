using UnityEngine;

public struct AnimatorState
{
    // Current Direction.
    public Vector2 Direction { get; private set; }

    // Last Direction of one of: (1,0), (1, 1), (1, -1), (0, 1), (0, -1), (-1, -1), (-1, 1)
    public Vector2 LastFullDirection { get; private set; }

    public State State { get; private set; }

    public AnimatorState(Vector2 direction, Vector2 lastFullDirection, State state)
    {
        this.Direction = direction;
        this.LastFullDirection = lastFullDirection;
        this.State = state;
    }
}
