using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StateComponent))]
public class EnemyAIComponent : InputController
{
    [SerializeField] private CombatType combatType;
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
        //timer -= Time.deltaTime;
        //Random_Move();

        // For testing
        Targetted_Ranged_Move(GameObject.Find("Player"), 5f);
    }

    // Randomly move around
    private void Random_Move()
    {
        // If the timer reaches or goes below 0, change movement direction
        if (timer <= 0f)
        {
            // Randomly move (randX = -1/0/1, randY = -1/0/1)
            int randX = UnityEngine.Random.Range(-1, 2); // Include 2 to generate numbers from -1 to 1
            int randY = UnityEngine.Random.Range(-1, 2);

            Vector2 dir = new Vector2(randX, randY);

            // Invoke the event to notify listeners about the new motion direction
            base.InvokeMovementInput(dir);

            // Debug.Log("Setting enemy dir to: " + dir);

            // Set state to WALKING (handle state in State component later)
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

    // Move torwards a target and attack (melee)
    private void Targetted_Move(GameObject target)
    {

    }

    // Move torwards a target and attack (ranged)
    // MinDist is the minimum distance to keep from the target
    private void Targetted_Ranged_Move(GameObject target, float minDist)
    {
        float dist = Vector2.Distance(transform.position, target.transform.position);
        Vector2 dir = target.transform.position - transform.position;
        if (dist < minDist)
        {
            // Move away from the target
            base.InvokeMovementInput(-dir);

        }
        else
        {
            // Move towards the target
            base.InvokeMovementInput(dir);
        }
    }
}



