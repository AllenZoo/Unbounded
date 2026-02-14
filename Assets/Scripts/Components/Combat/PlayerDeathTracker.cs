using NUnit.Framework;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// This script attaches to the player character and tracks their deaths throughout a run.
/// When player dies, it triggers a global OnPlayerDeath event that the GameManager listens to in order to end the run and show the game over screen.
/// </summary>
public class PlayerDeathTracker : MonoBehaviour
{
    [Required, SerializeField] private LocalEventHandler leh;
    private LocalEventBinding<OnDeathEvent> playerDeathBinding;

    private void Awake()
    {
        if (leh == null) leh = InitializerUtil.FindComponentInParent<LocalEventHandler>(gameObject);

        Assert.IsNotNull(leh, $"PlayerDeathTracker on {gameObject.name} requires a LocalEventHandler component in its parent hierarchy.");
    }


    private void Start()
    {
        // Register for player death event
        playerDeathBinding = new LocalEventBinding<OnDeathEvent>(OnPlayerDeath);
        leh.Register<OnDeathEvent>(playerDeathBinding);
    }

    private void OnEnable()
    {
        if (playerDeathBinding != null)
        {
            leh.Register<OnDeathEvent>(playerDeathBinding);
        }
    }

    private void OnDisable()
    {
        if (playerDeathBinding != null)
        {
            leh.Unregister<OnDeathEvent>(playerDeathBinding);
        }
    }

    private void OnPlayerDeath()
    {
        Debug.Log("PlayerDeathTracker: Player has died. Triggering OnPlayerDeath event.");
        EventBus<OnPlayerDeathEvent>.Call(new OnPlayerDeathEvent());
    }
}
