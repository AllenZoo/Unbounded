using UnityEngine;

/// <summary>
/// Attached to boss portal. Singleton since there should only be one of this object
/// </summary>
[RequireComponent(typeof(SceneTeleporter))]
public class BossPortalComponent : Singleton<BossPortalComponent>
{
    private SceneTeleporter sceneTeleporter; 

    protected override void Awake()
    {
        base.Awake();

        // TODO: Check if another BossPortalComponent exists. Log error message if there is
        sceneTeleporter = GetComponent<SceneTeleporter>();
       
    }

    public void SetBossTeleportation(SceneField scene)
    {
        sceneTeleporter.SetTargetScene(scene);
    }
}
