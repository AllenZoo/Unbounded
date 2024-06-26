using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour, ISpawner
{
    [SerializeField] LocalEventHandler localEventHandler;
    [SerializeField] private SpawnRates spawnRates;

    [Tooltip("Central Position to spawn enemies at.")]
    [SerializeField] private Transform spawnPos;

    [Tooltip("Max amount of enemies that can be spawned at once with this spawner.")]
    [SerializeField] private float maxSpawns = 5;

    [SerializeField] private float timeBetweenSpawns;
    
    private float timeSinceLastSpawn = 0;
    private List<Spawnable> spawns;

    // Controls whether spawner can spawn enemies. Depends on lastTime the spawner spawned or whether it currrently has max spawns.
    private bool canSpawn = true;

    // Controls whether to spawn or not. Usually depends on aggroStatus.
    private bool shouldSpawn = false;

    private void Awake()
    {
        if (spawnRates == null)
        {
            Debug.LogError("No spawn rates set for spawner.");
        }

        if (spawnPos == null)
        {
            Debug.LogError("No spawn position set for spawner. Set to default.");
            spawnPos = transform;
        }

        if (localEventHandler == null)
        {
            localEventHandler = GetComponentInParent<LocalEventHandler>();
            if (localEventHandler == null)
            {
                Debug.LogError("LocalEventHandler unassigned and not found in parent for object [" + gameObject +
                     "] with root object [" + gameObject.transform.root.name + "] for EnemySpawner.cs");
            }
        }
    }

    private void Start()
    {
        spawns = new List<Spawnable>();

        LocalEventBinding<OnDespawnEvent> onDespawnBinding = new LocalEventBinding<OnDespawnEvent>(RemoveContainedSpawnable);
        localEventHandler.Register(onDespawnBinding);

        LocalEventBinding<OnAggroStatusChangeEvent> onAggroStatusBinding = new LocalEventBinding<OnAggroStatusChangeEvent>(OnAggroStatusChangeEvent);
        localEventHandler.Register(onAggroStatusBinding);
    }

    /// <summary>
    /// Spawns the max amount that the spawner can spawn if it can spawn.
    /// </summary>
    public void MaxSpawn()
    {
        if (canSpawn)
        {
            Spawn(spawnPos.position, maxSpawns);
            timeSinceLastSpawn = 0f;
        }
    }

    /// <summary>
    /// Spawn an enemy object at given position
    /// </summary>
    /// <param name="pos"></param>
    /// <returns>Spawned enemy</returns>
    public GameObject Spawn(Vector2 pos, GameObject spawn)
    {
        return Instantiate(spawn, pos, Quaternion.identity);
    }

    /// <summary>
    /// Spawns the given amount (capped at maxSpawns - curSpawn) of enemies to spawn around a given position.
    /// </summary>
    /// <param name="centerPos"> center of position of spawned enemies</param>
    /// <param name="amount"> amount of enemies to spawn </param>
    /// <returns>List of spawned enemies</returns>
    public List<Spawnable> Spawn(Vector2 centerPos, float amount)
    {
        float curMaxAmount = Mathf.Clamp(amount, 0, maxSpawns - spawns.Count);
        List<GameObject> objsToSpawn = GetSpawnList(spawnRates, curMaxAmount);
        foreach (GameObject obj in objsToSpawn)
        {
            Vector2 curSpawnPos = centerPos + new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            GameObject spawn = Spawn(curSpawnPos, obj);
            Spawnable spawnComp = spawn.GetComponentOrInChildren<Spawnable>();
            spawnComp.SetSpawnerLocalEventHandler(localEventHandler);
            spawnComp.TriggerOnSpawn();
            spawns.Add(spawnComp);
        }

        return spawns;
    }

    /// <summary>
    /// Given a spawn rate and number of enemies to spawn, return a list of enemies to spawn.
    /// </summary>
    /// <param name="spawnRates"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    private List<GameObject> GetSpawnList(SpawnRates spawnRates, float amount)
    {
        List<GameObject> spawnList = new List<GameObject>();
        foreach (SpawnRate spawnRate in spawnRates.data.spawnRates)
        {
            // Check if reached max spawn capacity.
            if (spawnList.Count >= amount)
            {
                break;
            }

            // Iterate through min spawn of each rate.
            for (int i = 0; i < spawnRate.minSpawn; i++)
            {
                // Add the spawn to the list.
                spawnList.Add(spawnRate.prefab);
            }
        }

        int curCount = spawnList.Count;
        for (int i = 0; i < amount - curCount; i++)
        {
            // Draw a spawn from the spawn rates and add it to the list.
            spawnList.Add(DrawSpawn(spawnRates));
        }

        return spawnList;
    }

    /// <summary>
    /// Draw a spawn from the given spawn rates.
    /// </summary>
    /// <param name="spawnRates"></param>
    /// <returns></returns>
    private GameObject DrawSpawn(SpawnRates spawnRates)
    {
        float totalRate = 0;
        // Calculate total rate
        foreach (SpawnRate spawnRate in spawnRates.data.spawnRates)
        {
            totalRate += spawnRate.spawnRate;
        }

        // Random draw
        float draw = Random.Range(0, totalRate);

        // Iterate through spawn rates to find the spawn.
        float currentRate = 0;
        foreach (SpawnRate spawnRate in spawnRates.data.spawnRates)
        {
            currentRate += spawnRate.spawnRate;
            if (draw <= currentRate)
            {
                return spawnRate.prefab;
            }
        }
        
        // Should not reach here.
        Debug.LogError("Unreachable Error: No spawn drawn.");
        return null;
    }

    /// <summary>
    /// Remove the given spawnable from the list of current owned spawns.
    /// </summary>
    /// <param name="spawnable"></param>
    private void RemoveContainedSpawnable(OnDespawnEvent e)
    {
        spawns.Remove(e.spawn);
    }

    private void OnAggroStatusChangeEvent(OnAggroStatusChangeEvent e)
    {
        shouldSpawn = e.isAggroed;
    }

    private void Update()
    {
        if (shouldSpawn)
        {
            MaxSpawn();
        }

        if (timeSinceLastSpawn < timeBetweenSpawns)
        {
            timeSinceLastSpawn += Time.deltaTime;
            canSpawn = false;
        }
        else
        {
            canSpawn = true;
        } 
    }
}


public static class GameObjectExtensions
{
    /// <summary>
    /// Get the component of type T from the game object or its children.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public static T GetComponentOrInChildren<T>(this GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();
        if (component == null)
        {
            component = gameObject.GetComponentInChildren<T>();
        }
        return component;
    }
}
