using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Collider2D))]
// Should be Attached to every view collider of every entity.
// Helps manage sprite layering.
public class ViewColliderCollisionManager : MonoBehaviour
{
    [SerializeField] private Transform parentTransform;
    [SerializeField] private bool shouldFadeIfInFront = false;
    [SerializeField] private ObjectFader objFader;
    [SerializeField] private SpriteRenderer objSprite;

    [Tooltip("Whether the object should always be under.")]
    [HideIf(nameof(alwaysAbove))]
    [SerializeField] private bool alwaysUnder = false;

    [Tooltip("Whether the object should always be above.")]
    [HideIf(nameof(alwaysUnder))]
    [SerializeField] private bool alwaysAbove = false;

    [HideIf("@this.alwaysUnder || this.alwaysAbove")]
    [Tooltip("Position to be considered the objects feet relative to parent transform.")]
    [SerializeField] private Transform objFeet;

    [Header("For debugging, don't set value")]
    [SerializeField, ReadOnly] private int oldSortingOrder;
    private int bestSortingOrder;

    // TODO: use this to unfade, and reset sorting order when collidedWith.Count is empty.
    private HashSet<ViewColliderCollisionManager> collidedWith = new HashSet<ViewColliderCollisionManager>();
    private bool isMovingObject = false;

    private void Awake()
    {
        if (shouldFadeIfInFront)
        {
            // Try getting objFader from parent.
            if (objFader == null)
            {
                objFader = parentTransform.GetComponent<ObjectFader>();
            }
            Assert.IsNotNull(objFader);
        }

        Assert.IsNotNull(parentTransform);
        Assert.IsNotNull(objSprite);

        if (!alwaysAbove && !alwaysUnder) Assert.IsNotNull(objFeet);

        // Check if this object is on the ViewCollider layer.
        Assert.IsTrue(gameObject.layer == LayerMask.NameToLayer("ViewCollider"), "ViewColliderCollisionManager should be on the ViewCollider layer.");
    
        // Check if collider is a trigger.
        Assert.IsTrue(GetComponent<Collider2D>().isTrigger, "ViewColliderCollisionManager should have a trigger collider.");
    }

    private void Start()
    {
        oldSortingOrder = objSprite.sortingOrder;
        bestSortingOrder = oldSortingOrder;

        // Make Collider a trigger.
        GetComponent<Collider2D>().isTrigger = true;

        //collidedWith = new HashSet<ViewColliderCollisionManager>(); --> Moved this to initialize at runtime, since sometimes, it is possible that OnTriggerEnter2D gets called before this set is initialized.
        isMovingObject = parentTransform.CompareTag("Player") || parentTransform.CompareTag("Entity");
    }

    #region Public API
    /// <summary>
    /// Functions for dynamically changing whether an object should be always under etc.
    /// </summary>
    public void Sink()
    {
        alwaysAbove = false;
        alwaysUnder = true;

    }
    public void Rise()
    {
        alwaysUnder = false;
        alwaysAbove = true;
    }
    public void ResetSinkRise()
    {
        alwaysUnder = false;
        alwaysAbove = false;
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collidedWith.RemoveWhere(vccm => vccm == null);
        ProcessCollision(collision);
    }

    // Same behavior as Enter. This is here to check if player y has changed and whether object render behaviour should change.
    // Essentially an Update() but less expensive.
    private void OnTriggerStay2D(Collider2D collision)
    {
        collidedWith.RemoveWhere(vccm => vccm == null);
        ProcessCollision(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ViewColliderCollisionManager otherVCCM = collision.GetComponent<ViewColliderCollisionManager>();
        Assert.IsNotNull(otherVCCM, "Collision EXIT in ViewCollider layer should result in both objects containing" +
            "a ViewColliderCollisionManager component.");

        // Remove from collision list
        collidedWith.RemoveWhere(vccm => vccm == null);
        collidedWith.Remove(otherVCCM);

        // Check if any colliders in list are moving objects.
        foreach (ViewColliderCollisionManager vccm in collidedWith)
        {
            if (vccm.isMovingObject)
            {
                // Found a collider still colliding. So don't unfade.
                return;
            }
        }

        // Unfade
        if (objFader != null)
        {
            this.objFader.setDoFade(false);
        }

        // Set sorting order to old order before any collisions between static and dynamic view colliders.
        // Static = objects that don't move
        // Dynamic = objects that move
        if (isMovingObject)
        {
            // For moving VCCMs
            objSprite.sortingOrder = oldSortingOrder;
        }
        else
        {
            // For static
            objSprite.sortingOrder = bestSortingOrder;
        }
    }

    // Process collision (HELPER)
    // Checks whether collision belongs to a Player/Entity object and then passes it to 
    // HandleViewCollision() function.
    private void ProcessCollision(Collider2D collision)
    {
        // Collided object should be a ViewCollider and thus should also have a ViewColliderCollisionManager
        ViewColliderCollisionManager otherVCCM = collision.GetComponent<ViewColliderCollisionManager>();
        Assert.IsNotNull(otherVCCM, "Collision ENTER/STAY in ViewCollider layer should result in both objects containing" +
            "a ViewColliderCollisionManager component.");

        collidedWith.Add(otherVCCM);
        HandleViewCollision(otherVCCM);

        // If not colliding with any moving objects, set bestSortingOrder to cur sorting order.
        foreach (ViewColliderCollisionManager vccm in collidedWith)
        {
            if (vccm.isMovingObject)
            {
                // Found a collider still colliding. So don't set best sorting order.
                return;
            }
        }

        // Set best sorting order to current sorting order. (bso represents the best sorting order for all static view collisions) 
        bestSortingOrder = objSprite.sortingOrder;
    }

    /// <summary>
    /// If player is behind object, and object should fade,increase it's sorting layer so it renders in front of the player and is transparent.
    /// If againstEntity = true, the otherVCCM is from an Entity/Player. Else it is from another static object.
    /// </summary>
    /// <param name="otherVCCM"></param>
    /// <param name="againstEntity"></param>
    private void HandleViewCollision(ViewColliderCollisionManager otherVCCM)
    {
        // Check which feet is in front.
        float otherYPos = otherVCCM.GetFeetY();
        float thisYPos = GetFeetY();

        if (otherYPos > thisYPos)
        {
            // This object is in front of other.

            // Increase Sorting Layer of object if necessary.
            if (objSprite.sortingOrder <= otherVCCM.objSprite.sortingOrder)
            {
                // Increase
                objSprite.sortingOrder = otherVCCM.objSprite.sortingOrder + 1;

                // Since sorting order has changed, update sorting order for other colliding vccms
                foreach (ViewColliderCollisionManager vccm in collidedWith)
                {
                    if (vccm != this)
                    {
                        vccm.HandleViewCollision(this);
                    }
                }
            }
        }

        if (shouldFadeIfInFront)
        {
            // If shouldFadeIfInFront == true, objFader should not be null.
            objFader.setDoFade(ShouldCurFade());
        }
    }

    // Helper that checks if any moving objects are behind this object.
    private bool ShouldCurFade()
    {
        // Check if any colliders in list are moving objects.
        foreach (ViewColliderCollisionManager vccm in collidedWith)
        {
            float thisYPos = GetFeetY();
            float otherYPos = vccm.GetFeetY();
            
            if (vccm.isMovingObject && otherYPos > thisYPos)
            {
                // Found a collider still colliding and in front. So should fade.
                return true;
            }
        }

        // No moving objects in front. So don't fade.
        return false;
    }

    /// <summary>
    /// Helper to get FeetY depending on certain set parameters.
    /// </summary>
    /// <returns></returns>
    private float GetFeetY()
    {
        if (alwaysUnder)
        {
            return Mathf.Infinity;

        }
        else if (alwaysAbove)
        {
            return -Mathf.Infinity;
        }
        else
        {
            return objFeet.position.y;
        }
    }
}