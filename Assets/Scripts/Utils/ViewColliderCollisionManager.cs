using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Collider2D))]
public class ViewColliderCollisionManager : MonoBehaviour
{
    [SerializeField] private bool shouldFadeInFrontOfPlayer = false;

    [SerializeField] private int oldSortingOrder;

    private ObjectFader objFader;
    private SpriteRenderer objSprite;

    private void Awake()
    {
        if (shouldFadeInFrontOfPlayer)
        {
            Assert.IsNotNull(GetComponentInParent<ObjectFader>());
            objFader = GetComponentInParent<ObjectFader>();
        }
        
        Assert.IsNotNull(GetComponentInParent<SpriteRenderer>());
        objSprite = GetComponentInParent<SpriteRenderer>();
    }

    private void Start()
    {
        oldSortingOrder = objSprite.sortingOrder;
    }

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
        // Should fade, if object is in front of player.
        if (transform.parent.position.y < playerCollision.transform.parent.position.y)
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