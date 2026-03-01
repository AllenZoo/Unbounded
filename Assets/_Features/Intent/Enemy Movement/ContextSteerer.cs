using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

// Used to calculate the best direction for object to move in to get to a target pos. (Doesn't handle where the target is last seen, etc.)
// Generally used by EnemyAIComponent.
public class ContextSteerer : MonoBehaviour
{
    #region Debugging Variables
    [FoldoutGroup("Debugging")]
    [SerializeField] private float distanceThreshold = 0f; // TODO: temp variable, to play around with. After testing make this a constant such that all bosses have same value.
    [FoldoutGroup("Debugging")]
    [SerializeField] private bool log = false;
    [FoldoutGroup("Debugging")]
    [SerializeField] private bool showRays = false; // For debugging, to show the rays being cast to detect obstacles.
    public double[] TargetDirWeights => targetDirWeights;
    public double[] DangerDirWeights => dangerDirWeights;
    #endregion

    [Tooltip("Used to specify what layers raycast will interact with.")]
    [SerializeField] LayerMask obstaclesLayermask;

    [Tooltip("How far left and right from center of feet to also detect for danger weights. Essentially makes detection dirs \"thicker\".")]
    [SerializeField] private float dangerDirCurPosOffsets = 0.185f;

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
    public Vector2[] Directions => directions;
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
        CalculateDangerWeights(GetOffsetVector(currPos, dangerDirCurPosOffsets, Axis.X));
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
        CalculateDangerWeights(GetOffsetVector(currPos, dangerDirCurPosOffsets, Axis.X));
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
                // Assign a small random weigt (not zero) to directions that are opposite to targetDir, to add some randomization and help get out of sticky situations (e.g. when surrounded by obstacles).
                float random = Random.Range(0.25f, 0.55f);
                targetDirWeights[System.Array.IndexOf(directions, dir)] = random;
            }
            else
            {
                // Else, assign a weight of 1 - (angle / 90)
                targetDirWeights[System.Array.IndexOf(directions, dir)] = 1;// - (angle / 90);
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

    private double[] CalculateDangerWeights(List<Vector2> dangerPositions)
    {
        // For each danger position, calculate the danger dir weights.
        // Then take the max weight for each direction if there are multiple danger positions.

        double[] maxWeights = new double[16];
        foreach (Vector2 dangerPos in dangerPositions)
        {
            double[] weights = CalculateDangerDirWeights(dangerPos);

            // Update maxWeights array
            for (int i = 0; i < weights.Length; i++)
            {
                maxWeights[i] = Mathf.Max((float) maxWeights[i], (float) weights[i]);
            }
        }

        dangerDirWeights = maxWeights;
        return maxWeights;
    }
    private double[] CalculateDangerDirWeights(Vector2 currPos)
    {
        // Use raycasting to detect obstacles or dangers
        // and assign weights based on the level of danger and dist to danger
        // (e.g. if danger is close, assign a higher weight)

        var weights = new double[16];

        // Define ray length
        float rayLength = 5f;

        foreach (Vector2 dir in directions)
        {
            // Cast a ray and check for collisions
            // CurrPos is generally = feet.transform for bosses.
            Vector2 rayPos = currPos;

            // Get all raycast hits. Note that order is undefined.
            RaycastHit2D[] hits = Physics2D.RaycastAll(rayPos, dir, rayLength, obstaclesLayermask);
            RaycastHit2D hit = Physics2D.Raycast(rayPos, dir, rayLength, obstaclesLayermask); // Assign to avoid compile errors. This value will be overwritten in bottom loop.

            // Pick closest hit that is not self (aka. this.transform.root)
            foreach (RaycastHit2D _hit in hits)
            {
                var closestDist = _hit.distance;
                if (_hit.collider == null || _hit.collider.transform.root != transform.root)
                {
                    continue;
                } 

                if (_hit.distance < closestDist)
                {
                    hit = _hit;
                    closestDist = _hit.distance;
                }
            }


            // Draw ray for debugging
            if (showRays)
            {
                Debug.DrawRay(rayPos, dir * rayLength, Color.yellow, 1f);
            }
             
            // Avoid own hard collider by checking if hit.collider.transform.root is not the same as this.transform.root
            // (e.g. hard colliders usually have ObstacleCollider layer)
            if (hit.collider != null && hit.collider.transform.root != transform.root)
            {

                if (log)
                {
                    Debug.Log("Hit obstacle: " + hit.collider.name + " at distance: " + hit.distance + " in direction: " + dir);
                }
                
                // Adjust the danger weight based on the distance to the obstacle
                //double distanceFactor = rayLength -  (hit.distance);
                double distanceFactor = hit.distance;

                // ReLU (play around with threshold to achieve better behaviour)
                double weight = InverseThreshold(distanceFactor, distanceThreshold);
                weights[System.Array.IndexOf(directions, dir)] = weight;
            }
        }

        dangerDirWeights = weights;
        return weights;
    }

    private Vector2 CalculateBestDir()
    {

        // Calculate and store difference of two arrays: targetDirWeights - dangerDirWeights
        double[] weights = new double[16];

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
        Debug.DrawRay(transform.position, avgDir, Color.blue, 1f);

        return avgDir.normalized;
    }

    #region Activation Functions
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

    /// <summary>
    /// Activation function that is 1 when x < threshold, and then decreases as x increases past the threshold. Used to assign higher weights to directions that are safer (i.e. have a smaller distanceFactor value).
    /// </summary>
    /// <param name="x"></param>
    /// <param name="threshold"></param>
    /// <returns></returns>
    private double InverseThreshold(double x, double threshold)
    {
        if (x == 0)
        {
            return 1;
        }

        if (x < threshold)
        {
            return 1;
        } else
        {
            return (double) 1 / Mathf.Pow((float)x, 2);
        }
    }
    #endregion

    #region Helpers
    private List<Vector2> GetOffsetVector(Vector2 src, float offset, Axis axis)
    {
        List<Vector2> vectors = new List<Vector2>() { src };

        if (axis.Equals(Axis.X))
        {
            vectors.Add(new Vector2(src.x + offset, src.y));
            vectors.Add(new Vector2(src.x - offset, src.y));
        } else if (axis.Equals(Axis.Y))
        {
            vectors.Add(new Vector2(src.x, src.y + offset));
            vectors.Add(new Vector2(src.x, src.y - offset));
        }

            return vectors;
    }
    private void VisualizeVector(Vector2 vec, Color colour)
    {
        Debug.DrawRay(transform.position, vec, colour);
    }
    private void VisualizeVectors(List<Vector2> vectors, Color colour)
    {
        foreach (Vector2 vec in vectors)
        {
            Debug.DrawRay(transform.position, vec, colour);
        }
    }
    #endregion

    private void Update()
    {
        if (showRays)
        {
            // Show best direction ray for debugging
            Vector2 bestDir = CalculateBestDir();
            Debug.DrawRay(transform.position, bestDir.normalized * 5f, Color.blue);
        }
    }
}
