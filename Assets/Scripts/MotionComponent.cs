using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionComponent : MonoBehaviour
{
    [SerializeField] public Vector2 dir { get; private set; }
    private PlayerInput playerInput;
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        dir = new Vector2 (0, 0);
    }

    private void Start()
    {
        if (playerInput != null)
        {
            playerInput.OnMovementInput += PlayerInput_OnMovementInput;
        }
    }

    private void PlayerInput_OnMovementInput(Vector2 dir)
    {
        this.dir = dir;
    }
}
