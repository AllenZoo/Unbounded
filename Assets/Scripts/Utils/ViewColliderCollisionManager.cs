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

    [SerializeField] private bool shouldFadeInFrontOfPlayer = false;

    [SerializeField] private int oldSortingOrder;

    [SerializeField] private ObjectFader objFader;
    
    [SerializeField] private SpriteRenderer objSprite;

    [Tooltip("Position to be considered the objects feet relative to parent transform.")]
    [SerializeField] private Transform objFeet;

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
        if (shouldFadeInFrontOfPlayer)
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
        // View Collider just collided with View Collider.
        if (collision.gameObject.CompareTag("ViewCollider"))
        {
            // Check if collided View Collider belongs to the Player.
            Assert.IsNotNull(collision.transform.parent, "Parent of ViewCollider cannot be null.");
            if (collision.transform.parent.CompareTag("Player") && shouldFadeInFrontOfPlayer)
            {
                HandlePlayerViewCollision(collision);
            }
        }
    }

    // Same behavior as Enter. This is here to check if player y has changed and whether object render behaviour should change.
    // Essentially an Update() but less expensive.
    private void OnTriggerStay2D(Collider2D collision)
    {
        // View Collider just collided with View Collider.
        if (collision.gameObject.CompareTag("ViewCollider"))
        {
            // Check if collided View Collider belongs to the Player.
            Assert.IsNotNull(collision.transform.parent, "Parent of ViewCollider cannot be null.");
            if (collision.transform.parent.CompareTag("Player") && shouldFadeInFrontOfPlayer)
            {
                HandlePlayerViewCollision(collision);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // View Collider just exited View Collider.
        if (collision.gameObject.CompareTag("ViewCollider"))
        {
            // Check if collided View Collider belongs to the Player.
            Assert.IsNotNull(collision.transform.parent, "Parent of ViewCollider cannot be null.");
            if (collision.transform.parent.CompareTag("Player"))
            {
                Assert.IsNotNull(GetComponentInParent<ObjectFader>());
                GetComponentInParent<ObjectFader>().setDoFade(false);
            }
        }   
    }

    

    // If player is behind object, and object should fade,
    // increase it's sorting layer so it renders in front of the player and is transparent.
    private void HandlePlayerViewCollision(Collider2D playerCollision)
    {
        // TODO: once refactored, remove theses lines as otherVCCM should be passed as a function parameter.
        // Collided object should be a viewCollider and thus should also have a ViewColliderCollisionManager
        ViewColliderCollisionManager otherVCCM = playerCollision.GetComponent<ViewColliderCollisionManager>();
        Assert.IsNotNull(otherVCCM, "Collision in ViewCollider layer should result in both objects containing" +
            "a ViewColliderCollisionManager component.");

        // Should fade, if object is in front of player. 

        // Check which feet is in front.
        float playerYPos = otherVCCM.ObjFeet.position.y;
        float thisYPos = objFeet.position.y;

        if (playerYPos > thisYPos)
        {
            // Object is in front of player.

            // Increase Sorting Layer of Object
            objSprite.sortingOrder = playerCollision.transform.parent.GetComponentInParent<SpriteRenderer>().sortingOrder + 1;

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