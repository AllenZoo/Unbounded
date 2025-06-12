using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

// Sub-trees of attacks
// Check sub-tree if any attack is not active. return that attack to reuse.
// Singleton
public class AttackPool : MonoBehaviour
{
    public static AttackPool Instance { get; private set; }

    [SerializeField] private GameObject subPoolPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } else if (Instance != this)
        {
            Debug.LogError($"Destroying duplicate attack pool instance! This shouldn't happen if scene management is done properly." +
                $" Make sure that AttackPool obj isn't in anything but an object in PersistentGameplay scene.");
            Destroy(this);
        }

        if (subPoolPrefab == null)
        {
            subPoolPrefab = Instantiate(new GameObject(), this.transform);
        }
    }

    // Check if any attack is not active. return that attack to reuse.
    // If all attacks are active, instantiate new attack.
    // If this is the first attack of that kind, create a sub-pool.
    public GameObject GetAttack(GameObject attack)
    {
        Transform subPool = FindSubPool(attack).transform;
        for (int i = 0; i < subPool.childCount; i++)
        {
            var child = subPool.GetChild(i);

            if (!child.gameObject.activeSelf)
            {
                // Clear trailrenderers if it has
                var trailRenderComponent = child.GetComponent<TrailRenderer>();
                if (trailRenderComponent != null)
                {
                    trailRenderComponent.Clear();
                }

                return subPool.GetChild(i).gameObject;
            }
        }

        GameObject newAttack = Instantiate(attack, subPool);
        return newAttack;
    }

    // A sub-pool is a group of attacks of the same type.
    // This function will search for an existing sub-pool.
    // If it doesn't exist, it will create a new one.
    private GameObject FindSubPool(GameObject attack)
    {
        string groupName = attack.GetComponent<AttackComponent>().Attack.AttackData.attackName;
        Assert.IsTrue(groupName.EndsWith("|"), "It is required convention that attack names need to end with |." +
            "Cur attack name: " + groupName);

        // Attempt to search for existing sub-pool
        for (int i = 0; i < transform.childCount; i++)
        {
            // Debug.Log("Child Name: " + transform.GetChild(i).name);
            if (transform.GetChild(i).gameObject.name.Contains(groupName))
            {
                return transform.GetChild(i).gameObject;
            }
        }

        // Couldn't find existing sub-pool, create new one.
        GameObject newSubPool = Instantiate(subPoolPrefab, transform);
        newSubPool.name = groupName + " Pool";
        return newSubPool;
    }
}
