using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Collider2D))]
// Attached to every view collider of every entity.
public class ViewColliderCollisionManager : MonoBehaviour
{
    [SerializeField] private Transform parentTransform;
    [SerializeField] private bool shouldFadeIfInFront = false;
    [SerializeField] private ObjectFader objFader;
    [SerializeField] private SpriteRenderer objSprite;

    [Tooltip("Position to be considered the objects feet relative to parent transform.")]
    [SerializeField] private Transform objFeet;

    [Header("For debugging, don't set value")]
    [SerializeField] private int oldSortingOrder;
    private int bestSortingOrder;

    // TODO: use this to unfade, and reset sorting order when collidedWith.Count is empty.
    private HashSet<ViewColliderCollisionManager> collidedWith;
    private bool isMovingObject = false;

    private void Awake()
    {
        if (shouldFadeIfInFront)
        {
            Assert.IsNotNull(GetComponentInParent<ObjectFader>());
            Assert.IsNotNull(objFader);
        }

        Assert.IsNotNull(parentTransform);
        Assert.IsNotNull(objSprite);
        Assert.IsNotNull(objFeet);

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

        collidedWith = new HashSet<ViewColliderCollisionManager>();
        isMovingObject = parentTransform.CompareTag("Player") || parentTransform.CompareTag("Entity");
    }

    // TODO: refactor
    //  - move into private helper (so don't have dup code in OnTriggerStay2D)
    //  - assign 'ViewCollider' layer and make it such that objs on 'ViewCollider' can only
    //    collide with other objects on 'ViewCollider' layer.
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
        // Check if collided View Collider belongs to the Player.
        ViewColliderCollisionManager otherVCCM = collision.GetComponent<ViewColliderCollisionManager>();
        Assert.IsNotNull(otherVCCM, "Collision EXIT in ViewCollider layer should result in both objects containing" +
            "a ViewColliderCollisionManager component.");

        collidedWith.RemoveWhere(vccm => vccm == null);
        collidedWith.Remove(otherVCCM);

        // Check if any collided View Colliders belongs to the Player/Entity.
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
        float otherYPos = otherVCCM.objFeet.position.y;
        float thisYPos = objFeet.position.y;

        if (otherYPos > thisYPos)
        {
            // Object is in front of other.

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
            
            // If other is a moving object (player/entity), fade if necessary.
            if (shouldFadeIfInFront && otherVCCM.isMovingObject)
            {
                // Fade Object
                objFader.setDoFade(true);
            }
        }
        else
        {
            // Object is behind other.

            // If other is a moving object (player/entity), and it is the only moving object... unfade if necessary.
            if (shouldFadeIfInFront && otherVCCM.isMovingObject)
            {
                // Undo Fade (if faded)
                objFader.setDoFade(false);
            }
            
        }
    }
}