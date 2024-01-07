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
    // TODO: use this to unfade, and reset sorting order when collidedWith.Count is empty.
    private List<ViewColliderCollisionManager> collidedWith;

    public ObjectFader ObjFader
    {
        get { return objFader; }
    }
    public SpriteRenderer ObjSprite
    {
        get { return objSprite; }
    }
    public Transform ParentTransform
    {
        get { return parentTransform;}
    }
    public Transform ObjFeet { 
        get { return objFeet; } 
    }

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

        // Make Collider a trigger.
        GetComponent<Collider2D>().isTrigger = true;
    }

    // TODO: refactor
    //  - move into private helper (so don't have dup code in OnTriggerStay2D)
    //  - assign 'ViewCollider' layer and make it such that objs on 'ViewCollider' can only
    //    collide with other objects on 'ViewCollider' layer.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        ProcessCollision(collision);
    }

    // Same behavior as Enter. This is here to check if player y has changed and whether object render behaviour should change.
    // Essentially an Update() but less expensive.
    private void OnTriggerStay2D(Collider2D collision)
    {
        ProcessCollision(collision);
    }

    // Undo fade if object leaving the ViewCollider is player/entity
    // POTENTIAL BUG: If player leaves the collider, but there is an entity there, the object will
    // be opaque and then fade once another OnTriggerStay is called.
    private void OnTriggerExit2D(Collider2D collision)
    {
        // Check if collided View Collider belongs to the Player.
        ViewColliderCollisionManager otherVCCM = collision.GetComponent<ViewColliderCollisionManager>();
        Assert.IsNotNull(otherVCCM, "Collision EXIT in ViewCollider layer should result in both objects containing" +
            "a ViewColliderCollisionManager component.");

        // Check if collided View Collider belongs to the Player/Entity.
        if (otherVCCM.parentTransform.CompareTag("Player")
            || otherVCCM.parentTransform.CompareTag("Entity"))
        {
            // Unfade
            if (objFader != null)
            {
                this.objFader.setDoFade(false);
            }
        }

        // Reset Sorting Layer of Object
        objSprite.sortingOrder = oldSortingOrder;
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

        // Check if collided View Collider belongs to the Player/Entity.
        if (otherVCCM.parentTransform.CompareTag("Player")
            || otherVCCM.parentTransform.CompareTag("Entity"))
        {
            HandleViewCollision(otherVCCM, true);
        } else
        {
            HandleViewCollision(otherVCCM, false);
        }
    }

    /// <summary>
    /// If player is behind object, and object should fade,increase it's sorting layer so it renders in front of the player and is transparent.
    /// If againstEntity = true, the otherVCCM is from an Entity/Player. Else it is from another static object.
    /// </summary>
    /// <param name="otherVCCM"></param>
    /// <param name="againstEntity"></param>
    private void HandleViewCollision(ViewColliderCollisionManager otherVCCM, bool againstEntity)
    {
        // Check which feet is in front.
        float otherYPos = otherVCCM.ObjFeet.position.y;
        float thisYPos = objFeet.position.y;

        if (otherYPos > thisYPos)
        {
            // This Object is in front of other (player/entity).

            // Increase Sorting Layer of Object
            objSprite.sortingOrder = otherVCCM.ObjSprite.sortingOrder + 1;


            if (shouldFadeIfInFront && againstEntity)
            {
                // Fade Object
                objFader.setDoFade(true);
            }
        }
        else
        {
            // Object is behind player.

            // Decrease Sorting Layer of Object
            objSprite.sortingOrder = otherVCCM.ObjSprite.sortingOrder - 1;


            if (shouldFadeIfInFront && againstEntity)
            {
                // Undo Fade (if faded)
                objFader.setDoFade(false);
            }
            
        }
    }
}