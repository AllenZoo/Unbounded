using UnityEngine;

// Script attached to object, which will teleport the player to this objects transform.
// Useful for after loading scene and needing the player to be at a certain position.
public class PlayerTeleporter : MonoBehaviour
{
    EventBinding<OnSceneLoadRequestFinish> sceneLoadFinishBinding;
    private bool hasTeleported = false;

    private void Awake()
    {
        sceneLoadFinishBinding = new EventBinding<OnSceneLoadRequestFinish>(TeleportPlayer);
        EventBus<OnSceneLoadRequestFinish>.Register(sceneLoadFinishBinding);
    }

    private void Start()
    {
        TeleportPlayer();
    }

    private void TeleportPlayer()
    {
        GameObject player = PlayerSingleton.Instance?.gameObject;
        if (player != null && !hasTeleported)
        {
            player.transform.position = transform.position;
            player.transform.rotation = transform.rotation;

            // Shouldn't modify collection in middle of the event calling everywhere else.
            // Causes issues.
            //EventBus<OnSceneLoadRequestFinish>.Unregister(sceneLoadFinishBinding);
            this.gameObject.SetActive(false);
            hasTeleported = true;
        }
    }
}
