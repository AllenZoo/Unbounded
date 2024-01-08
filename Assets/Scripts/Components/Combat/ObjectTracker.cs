using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Tracks the position of a target object via raycasts, and stores the lastSeenPos of the target.
public class ObjectTracker : MonoBehaviour
{
    [Tooltip("Layer of objects that could block line of sight to target.")]
    [SerializeField] private LayerMask obstacleLayerMask;

    [Header("Cone Raycast Settings")]
    [SerializeField] private float sightRange = 10f;
    [SerializeField] private int rayCount = 18;
    [SerializeField] private float coneAngle = 45f;


    private GameObject target;
    private Vector2 lastSeenTargetPos;
    private LayerMask targetLayerMask;
    
    public void Track(GameObject target)
    {
        this.target = target;
        targetLayerMask = LayerMask.GetMask(LayerMask.LayerToName(target.layer));
    }

    public Vector2 GetLastSeenTargetPos()
    {
        return lastSeenTargetPos;
    }


    private void Update()
    {
        if (target != null)
        {
            CastConeRays();
            // Debug.Log("Last seen target pos: " + lastSeenTargetPos);
        }
    }

    private void CastConeRays()
    {
        float angleStep = coneAngle / (rayCount - 1);
        float currentAngle = -coneAngle / 2; // Start from the left side of the cone

        for (int i = 0; i < rayCount; i++)
        {
            // Calculate the direction of the ray based on the current angle
            Vector2 rayDirection = Quaternion.Euler(0, 0, currentAngle) * (target.transform.position - (Vector3)transform.position);

            // Cast a ray
            RaycastHit2D targetHit = Physics2D.Raycast(transform.position, rayDirection, sightRange, targetLayerMask);
            // 
           
            // Debug draw to visualize the cone rays (optional)
            // Debug.DrawRay(transform.position, rayDirection.normalized * sightRange, Color.green);

            
            if (targetHit.collider != null)
            {
                // Hit the target! Check if there are any obstacles in the way.
                RaycastHit2D obstacleHit = Physics2D.Raycast(transform.position, rayDirection, targetHit.distance, obstacleLayerMask);

                if (obstacleHit.collider == null)
                {
                    // No obstacles in the way, so we can see the target
                    lastSeenTargetPos = target.transform.position;
                    break; // Break the loop if any ray hits the target
                }
                
            }

            // Increment the angle for the next ray
            currentAngle += angleStep;
        }
    }
}
