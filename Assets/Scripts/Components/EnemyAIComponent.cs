using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StateComponent))]
public class EnemyAIComponent : MonoBehaviour
{
    public event Action<Vector2> OnMotionChange;
    private StateComponent state;

    private float movementTimer = 3f; // Time interval to change movement direction
    private float timer;

    private void Awake()
    {
        state = GetComponent<StateComponent>();
    }

    private void Start()
    {
        // Set the initial timer value
        timer = movementTimer;
    }

    private void Update()
    {
        // Count down the timer
        timer -= Time.deltaTime;
        Handle_Move();
    }

    private void Handle_Move()
    {
        // If the timer reaches or goes below 0, change movement direction
        if (timer <= 0f)
        {
            // Randomly move (randX = -1/0/1, randY = -1/0/1)
            int randX = UnityEngine.Random.Range(-1, 2); // Include 2 to generate numbers from -1 to 1
            int randY = UnityEngine.Random.Range(-1, 2);

            Vector2 dir = new Vector2(randX, randY);

            // Invoke the event to notify listeners about the new motion direction
            OnMotionChange?.Invoke(dir);

            // Debug.Log("Setting enemy dir to: " + dir);

            // Set state to WALKING (handle state in helper later)
            if (randX != 0 || randY != 0)
            {
                state.SetState(State.WALKING);
            }
            else
            {
                state.SetState(State.IDLE);
            }

            // Reset the timer
            timer = movementTimer;
        }
    }
}



