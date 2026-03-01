using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script attached to object, which will teleport the player to this objects transform.
// Useful for after loading scene and needing the player to be at a certain position.
public class PlayerTeleporter : MonoBehaviour
{
    [Header("Spawn & Respawn Options")]
    [SerializeField, Tooltip("If the player is missing from the scene, should we spawn them?")]
    private bool spawnIfMissing = true;

    [SerializeField, Tooltip("The player prefab to spawn if missing.")]
    private GameObject playerPrefab;

    [SerializeField, Tooltip("If true, this teleporter will attempt to respawn the player if they are deactivated (death).")]
    private bool respawnOnDeath = false;

    [SerializeField, Tooltip("Delay in seconds before the player is respawned.")]
    private float respawnDelay = 1.0f;

    /// <summary>
    /// Static reference to the most recently active teleporter, used to determine which one handles respawning.
    /// </summary>
    private static PlayerTeleporter lastActiveTeleporter;

    private EventBinding<OnSceneLoadRequestFinish> sceneLoadFinishBinding;
    private EventBinding<OnPlayerDeathEvent> playerDeathBinding;
    private bool hasTeleported = false;
    private GameObject cachedPlayer;

    private void Awake()
    {
        sceneLoadFinishBinding = new EventBinding<OnSceneLoadRequestFinish>(TeleportPlayer);
        EventBus<OnSceneLoadRequestFinish>.Register(sceneLoadFinishBinding);

        playerDeathBinding = new EventBinding<OnPlayerDeathEvent>(HandlePlayerDeath);
    }

    private void OnEnable()
    {
        if (playerDeathBinding != null) EventBus<OnPlayerDeathEvent>.Register(playerDeathBinding);
        
        // If this teleporter is enabled during gameplay, it might become the new spawn point
        if (hasTeleported) lastActiveTeleporter = this;
    }

    private void OnDisable()
    {
        if (playerDeathBinding != null) EventBus<OnPlayerDeathEvent>.Unregister(playerDeathBinding);
    }

    private void Start()
    {
        lastActiveTeleporter = this;
        TeleportPlayer();
    }

    private void TeleportPlayer()
    {
        // Try to get player from singleton or cache
        GameObject player = PlayerSingleton.Instance?.gameObject;
        if (player == null) player = cachedPlayer;

        // Attempt to spawn if absolutely missing
        if (player == null && spawnIfMissing && playerPrefab != null)
        {
            player = Instantiate(playerPrefab, transform.position, transform.rotation);
            Debug.Log($"[PlayerTeleporter] Player missing. Spawned new instance from prefab at {transform.position}");
        }

        if (player != null)
        {
            cachedPlayer = player;

            // Reactivate player if it was deactivated (user requested reactivation instead of spawning)
            if (!player.activeSelf)
            {
                player.SetActive(true);

                // Notify systems of respawn so they can reset states, cooldowns, etc.
                // TODO: make this less coupled
                player.GetComponentInChildren<LocalEventHandler>().Call(new OnRespawnEvent());
                
                // TODO: move this logic to individual components so they can control how to handle resetting.
                //       Leave like this for now since it doesn't break anything.
                // Reset state to IDLE to allow movement/input again
                StateComponent stateComp = player.GetComponent<StateComponent>();
                if (stateComp != null) stateComp.ResetState();

                // Reset health to max is now handled by StatComponent via OnRespawnEvent.
            }

            if (!hasTeleported)
            {
                player.transform.position = transform.position;
                player.transform.rotation = transform.rotation;

                hasTeleported = true;
                lastActiveTeleporter = this;

                // If we don't need to respawn, we can deactivate the object as per original behavior.
                // If we do need to respawn, we must stay active to receive events.
                if (!respawnOnDeath)
                {
                    this.gameObject.SetActive(false);
                }
            }
        }
    }

    private void HandlePlayerDeath()
    {
        // Only the last teleporter the player encountered should handle the respawn to avoid multiple spawns.
        if (respawnOnDeath && lastActiveTeleporter == this)
        {
            StartCoroutine(RespawnRoutine());
        }
    }

    private IEnumerator RespawnRoutine()
    {
        Debug.Log($"[PlayerTeleporter] Player death detected. Reactivating in {respawnDelay} seconds...");
        yield return new WaitForSeconds(respawnDelay);
        
        hasTeleported = false;
        TeleportPlayer();
    }
}
