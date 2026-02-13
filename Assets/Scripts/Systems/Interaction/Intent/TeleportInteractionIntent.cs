using UnityEngine;

public class TeleportInteractionIntent : ICommittableInteraction
{
    readonly private SceneField targetScene;
    readonly private bool unloadAllButPersistent;

    public TeleportInteractionIntent(SceneField scene, bool unloadAllButPersistent = false)
    {
        targetScene = scene;
        this.unloadAllButPersistent = unloadAllButPersistent;
    }

    public void Commit()
    {
        EventBus<OnSceneTeleportRequest>.Call(new OnSceneTeleportRequest
        {
            targetScene = targetScene,
            unloadAllButPersistent = unloadAllButPersistent
        });
    }   

    public void Cancel()
    {
        // Intentionally empty – no side effects
    }
}
