using UnityEngine;

public class TeleportInteractionIntent : ICommittableInteraction
{
    readonly private SceneField targetScene;

    public TeleportInteractionIntent(SceneField scene)
    {
        targetScene = scene;
    }

    public void Commit()
    {
        EventBus<OnSceneTeleportRequest>.Call(new OnSceneTeleportRequest
        {
            targetScene = targetScene
        });
    }   

    public void Cancel()
    {
        // Intentionally empty – no side effects
    }
}
