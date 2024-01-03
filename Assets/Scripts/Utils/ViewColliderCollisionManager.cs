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

    [SerializeField] private int oldSortingOrder;

    [SerializeField] private ObjectFader objFader;
    
    [SerializeField] private SpriteRenderer objSprite;

    [Tooltip("Position to be considered the objects feet relative to parent transform.")]
    [SerializeField] private Transform objFeet;

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
    }

    private void Start()
    {
        oldSortingOrder = objSprite.sortingOrder;
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
            this.objFader.setDoFade(false);
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

        // Check if collided View Collider belongs to the Player/Entity.
        if (otherVCCM.parentTransform.CompareTag("Player")
            || otherVCCM.parentTransform.CompareTag("Entity"))
        {
            HandleViewCollision(otherVCCM);
        }
    }

    // If player is behind object, and object should fade,
    // increase it's sorting layer so it renders in front of the player and is transparent.
    // Make this more general and not just player.
    private void HandleViewCollision(ViewColliderCollisionManager otherVCCM)
    {
        // Check if should fade, if object is in front of otherVCCM
        if (!shouldFadeIfInFront)
        {
            // Shouldn't fade, so just return.
            return;
        }

        // Check which feet is in front.
        float playerYPos = otherVCCM.ObjFeet.position.y;
        float thisYPos = objFeet.position.y;

        if (playerYPos > thisYPos)
        {
            // Object is in front of player.

            // Increase Sorting Layer of Object
            objSprite.sortingOrder = otherVCCM.ObjSprite.sortingOrder + 1;

            // Fade Object
            objFader.setDoFade(true);
        }
        else
        {
            // Object is behind player.
            // Reset Sorting Layer of Object
            objSprite.sortingOrder = oldSortingOrder;

            // Undo Fade (if faded)
            objFader.setDoFade(false);
        }
    }
}