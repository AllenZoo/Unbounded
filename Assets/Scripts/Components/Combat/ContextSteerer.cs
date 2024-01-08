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
    // 0 = up, 1 = up-right, 2 = right, 3 = down-right, 4 = down, 5 = down-left, 6 = left, 7 = up-left,
    // 8 = up-right-intermediate, 9 = right-up-intermediate, 10 = right-down-intermediate, 11 = down-right-intermediate,
    // 12 = down-left-intermediate, 13 = left-down-intermediate, 14 = left-up-intermediate, 15 = up-left-intermediate
    private double[] targetDirWeights = new double[16];

    // Weights that represent if going in a certain direction will lead to danger. (0 = no danger, 1 = danger).
    // The order of the weight is as follows:
    // 0 = up, 1 = up-right, 2 = right, 3 = down-right, 4 = down, 5 = down-left, 6 = left, 7 = up-left,
    // 8 = up-right-intermediate, 9 = right-up-intermediate, 10 = right-down-intermediate, 11 = down-right-intermediate,
    // 12 = down-left-intermediate, 13 = left-down-intermediate, 14 = left-up-intermediate, 15 = up-left-intermediate
    private double[] dangerDirWeights = new double[16];

    // The directions that correspond to the weights above.
    private Vector2[] directions = new Vector2[16] {
        new Vector2(0, 1), // Up
        new Vector2(0.7071f, 0.7071f), // Up-right
        new Vector2(1, 0), // Right
        new Vector2(0.7071f, -0.7071f), // Down-right
        new Vector2(0, -1), // Down
        new Vector2(-0.7071f, -0.7071f), // Down-left
        new Vector2(-1, 0), // Left
        new Vector2(-0.7071f, 0.7071f), // Up-left
        new Vector2(0.3827f, 0.9239f), // Up-right-intermediate
        new Vector2(0.9239f, 0.3827f), // Right-up-intermediate
        new Vector2(0.9239f, -0.3827f), // Right-down-intermediate
        new Vector2(0.3827f, -0.9239f), // Down-right-intermediate
        new Vector2(-0.3827f, -0.9239f), // Down-left-intermediate
        new Vector2(-0.9239f, -0.3827f), // Left-down-intermediate
        new Vector2(-0.9239f, 0.3827f), // Left-up-intermediate
        new Vector2(-0.3827f, 0.9239f)  // Up-left-intermediate
    };

    /// <summary>
    /// Get the best direction for this object to move in to get TO the target pos.
    /// </summary>
    /// <param name="targetPos"></param>
    /// <param name="currPos"></param>
    /// <returns>A normalized direction vector</returns>
    public Vector2 GetDirTorwards(Vector2 targetPos, Vector2 currPos)
    {
        CalculateTargetDirWeights(targetPos, currPos, true);
        CalculateDangerDirWeights(currPos);
        Vector2 dir = CalculateBestDir();
        return dir;
    }

    /// <summary>
    /// Get the best direction for this object to move in to get AWAY from the target pos.
    /// </summary>
    /// <param name="targetPos"></param>
    /// <param name="currPos"></param>
    /// <returns></returns>
    public Vector2 GetDirAway(Vector2 targetPos, Vector2 currPos)
    {
        CalculateTargetDirWeights(targetPos, currPos, false);
        CalculateDangerDirWeights(currPos);
        Vector2 dir = CalculateBestDir();
        return dir;
    }

    /// <summary>
    /// Fill up the targetDirWeights array with weights based on the targetPos and whether we want to go
    /// torwards it or away from it.
    /// </summary>
    /// <param name="targetPos"></param>
    /// <param name="currPos"></param>
    /// <param name="shouldGoTorwards">If our weight should signify we go torwards or not.</param>
    private void CalculateTargetDirWeights(Vector2 targetPos, Vector2 currPos, bool shouldGoTorwards)
    {
        // Calculate weights based on the target direction
        Vector2 targetDir = (targetPos - currPos).normalized;

        foreach (Vector2 dir in directions)
        {
            // Calculate angle between targetDir and dir
            float angle = Vector2.Angle(targetDir, dir);
            
            // If angle > 90, then assign a weight of 0 (or maybe add some randomization to get out of sticky situations)
            if (angle > 90)
            {
                float random = Random.Range(0f, 0.2f);
                targetDirWeights[System.Array.IndexOf(directions, dir)] = random;
            }
            else
            {
                // Else, assign a weight of 1 - (angle / 90)
                targetDirWeights[System.Array.IndexOf(directions, dir)] = 1 - (angle / 90);
            }
        }

        if (!shouldGoTorwards)
        {
            // Invert the weights if we want to go away from the target
            for (int i = 0; i < targetDirWeights.Length; i++)
            {
                targetDirWeights[i] = 1 - targetDirWeights[i];
            }
        }
    }

    private void CalculateDangerDirWeights(Vector2 currPos)
    {
        // Use raycasting to detect obstacles or dangers
        // and assign weights based on the level of danger and dist to danger
        // (e.g. if danger is close, assign a higher weight)

        // Clear previous danger weights
        for (int i = 0; i < dangerDirWeights.Length; i++)
        {
            dangerDirWeights[i] = 0;
        }


        // Define ray length
        float rayLength = 5f;

        foreach (Vector2 dir in directions)
        {
            // Cast a ray and check for collisions

            float offset = 0.2f;
            Vector2 rayPos = currPos + dir * offset;

            RaycastHit2D hit = Physics2D.Raycast(rayPos, dir, rayLength, obstaclesLayermask);

            // Draw ray for debugging
            // Debug.DrawRay(rayPos, dir * rayLength, Color.red, 1f);

            if (hit.collider != null)
            {
                // Adjust the danger weight based on the distance to the obstacle
                

                double distanceFactor = rayLength -  (hit.distance);

                // ReLU (play around with threshold to achieve better behaviour)
                double weight = ReLU(distanceFactor, 4.65);

                dangerDirWeights[System.Array.IndexOf(directions, dir)] = weight;
            }
        }
    }

    private Vector2 CalculateBestDir()
    {

        // Calculate and store difference of two arrays: targetDirWeights - dangerDirWeights
        double[] weights = new double[8];

        for (int i = 0; i < weights.Length; i++)
        {
            // Clamp weight vals to [0, 1]
            weights[i] = targetDirWeights[i] - dangerDirWeights[i];
            weights[i] = Mathf.Clamp((float)weights[i], 0, 1);
        }

        // Use this weight array to determine the best direction to move in
        // Calculate the average of the weights * directions
        Vector2 avgDir = Vector2.zero;
        for (int i = 0; i < weights.Length; i++)
        {
            avgDir += (float) weights[i] * directions[i];
        }

        // Draw ray pointing in direction of avgDir for debugging
        // Debug.DrawRay(transform.position, avgDir, Color.red, 1f);

        return avgDir.normalized; // Stub
    }

    private double Sigmoid(float x)
    {
        return 1 / (1 + Mathf.Exp(-x));
    }

    private double ReLU(double x, double threshold)
    {
        if (x > threshold)
        {
            return x;
        }
        else
        {
            return 0;
        }
    }

    //private void Update()
    //{
    //    // For debugging
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        CalculateDangerDirWeights(transform.position);
    //    }
    //}
}
