using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used to calculate the best direction for object to move in to get to a target pos. (Doesn't handle where the target is last seen, etc.)
// Generally used by EnemyAIComponent.
public class ContextSteerer : MonoBehaviour
{
    [Tooltip("Used to specify what layers raycast will interact with.")]
    [SerializeField] LayerMask obstaclesLayermask;

    // Weights that represent how ideal each direction is for following target. (0 = not ideal, 1 = ideal). 
    // The order of the weight is as follows: 
    // 0 = up, 1 = up-right, 2 = right, 3 = down-right, 4 = down, 5 = down-left, 6 = left, 7 = up-left
    private double[] targetDirWeights = new double[8];

    // Weights that represent if going in a certain direction will lead to danger. (0 = no danger, 1 = danger).
    // The order of the weight is as follows:
    // 0 = up, 1 = up-right, 2 = right, 3 = down-right, 4 = down, 5 = down-left, 6 = left, 7 = up-left
    private double[] dangerDirWeights = new double[8];

    // The directions that correspond to the weights above.
    private Vector2[] directions = new Vector2[8] { 
        new Vector2(0, 1), new Vector2(1, 1).normalized, new Vector2(1, 0), new Vector2(1, -1).normalized,
        new Vector2(0, -1), new Vector2(-1, -1).normalized, new Vector2(-1, 0), new Vector2(-1, 1).normalized,};

    /// <summary>
    /// Get the best direction for this object to move in to get to the target pos.
    /// </summary>
    /// <param name="targetPos"></param>
    /// <param name="currPos"></param>
    /// <returns>A normalized direction vector</returns>
    public Vector2 GetDir(Vector2 targetPos, Vector2 currPos)
    {
        CalculateTargetDirWeights(targetPos, currPos);
        CalculateDangerDirWeights(currPos);
        Vector2 dir = CalculateBestDir();
        return dir;
    }

    private void CalculateTargetDirWeights(Vector2 targetPos, Vector2 currPos)
    {
        // Calculate weights based on the target direction
        Vector2 targetDir = (targetPos - currPos).normalized;

        foreach (Vector2 dir in directions)
        {
            // Calculate angle between targetDir and dir
            float angle = Vector2.Angle(targetDir, dir);
            
            // If angle > 90, then assign a weight of 0
            if (angle > 90)
            {
                targetDirWeights[System.Array.IndexOf(directions, dir)] = 0;
            }
            else
            {
                // Else, assign a weight of 1 - (angle / 90)
                targetDirWeights[System.Array.IndexOf(directions, dir)] = 1 - (angle / 90);
            }
        }
    }

    private void CalculateDangerDirWeights(Vector2 currPos)
    {
        // Use raycasting to detect obstacles or dangers
        // and assign weights based on the level of danger and dist to danger
        // (e.g. if danger is close, assign a higher weight)

        // Clear danger weights
        //for (int i = 0; i < dangerDirWeights.Length; i++)
        //{
        //    dangerDirWeights[i] = 0;
        //}


        //// Define ray length
        float rayLength = 5f;

        foreach (Vector2 dir in directions)
        {
            // Cast a ray and check for collisions
            RaycastHit2D hit = Physics2D.Raycast(currPos, dir, rayLength, obstaclesLayermask);
            if (hit.collider != null)
            {
                // Adjust the danger weight based on the distance to the obstacle
                // 1 = obstacle is right in front, 0 = obstacle is far
                // Make weight grow exponentially as distance decreases
                
                // Tweak
                float exponent = 5f;

                float distanceFactor = 1f - Mathf.Clamp01(hit.distance / rayLength);
                float exponentialFactor = Mathf.Pow(distanceFactor, exponent); // Use your desired exponent

                dangerDirWeights[System.Array.IndexOf(directions, dir)] = exponentialFactor;
            }
        }
    }

    private Vector2 CalculateBestDir()
    {
        // Calculate and store difference of two arrays: targetDirWeights - dangerDirWeights
        double[] weights = new double[8];

        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] = targetDirWeights[i] - dangerDirWeights[i];
        }


        // Use this weight array to determine the best direction to move in
        // Calculate the average of the weights * directions
        Vector2 avgDir = Vector2.zero;
        for (int i = 0; i < weights.Length; i++)
        {
            avgDir += (float) weights[i] * directions[i];
        }

        return avgDir.normalized; // Stub
    }
}
