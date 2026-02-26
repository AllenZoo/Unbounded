using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

/// <summary>
/// A testing script that allows for quick transitions into specific boss scenes.
/// Attach this to a GameObject (e.g., in a Debugging or Bootstrap scene).
/// </summary>
public class BossSceneTransitionTester : MonoBehaviour
{
    [Title("Boss Scene Settings")]
    [Tooltip("List of boss scenes. If empty, it will use the default boss scenes identified in the project.")]
    [SerializeField] private List<SceneField> bossScenes = new List<SceneField>();

    [Tooltip("If true, unloads all other scenes except persistent ones (Bootstrap, PersistentGameplay).")]
    [SerializeField] private bool unloadAllButPersistent = true;

    [Title("Quick Access Buttons")]
    [HorizontalGroup("BossButtons")]
    [Button("Azhurak's Pyre", ButtonHeight = 30), GUIColor(1f, 0.4f, 0.4f)]
    public void TeleportToAzhurak() => TeleportToScene("Azhurak's Pyre");

    [HorizontalGroup("BossButtons")]
    [Button("Snowfall Cavern", ButtonHeight = 30), GUIColor(0.4f, 0.7f, 1f)]
    public void TeleportToSnowfall() => TeleportToScene("Snowfall Cavern");

    [HorizontalGroup("BossButtons2")]
    [Button("Deadwind Cove", ButtonHeight = 30), GUIColor(0.5f, 0.5f, 0.5f)]
    public void TeleportToDeadwind() => TeleportToScene("Deadwind Cove");

    [HorizontalGroup("BossButtons2")]
    [Button("Deepwood Hollow", ButtonHeight = 30), GUIColor(0.4f, 0.8f, 0.4f)]
    public void TeleportToDeepwood() => TeleportToScene("Deepwood Hollow");

    [Button("Anchorpoint (Homebase)", ButtonHeight = 40), GUIColor(1f, 1f, 0.6f)]
    public void TeleportToHomebase() => TeleportToScene("Anchorpoint");

    [Title("Custom Scene")]
    [SerializeField] private SceneField customScene;

    [Button("Teleport to Custom Scene", ButtonHeight = 30)]
    public void TeleportToCustom() => TeleportToSelectedScene(customScene);

    /// <summary>
    /// Teleports the player to the specified scene by name.
    /// </summary>
    public void TeleportToScene(string sceneName)
    {
        TeleportToSelectedScene(new SceneField(sceneName));
    }

    /// <summary>
    /// Teleports the player to the specified SceneField.
    /// </summary>
    public void TeleportToSelectedScene(SceneField scene)
    {
        if (scene == null || string.IsNullOrEmpty(scene.SceneName))
        {
            Debug.LogWarning("[BossSceneTransitionTester] No scene specified!");
            return;
        }

        Debug.Log($"[BossSceneTransitionTester] Requesting teleport to: {scene.SceneName}");
        
        // Use the existing event system to trigger the teleport
        EventBus<OnSceneTeleportRequest>.Call(new OnSceneTeleportRequest
        {
            targetScene = scene,
            unloadAllButPersistent = unloadAllButPersistent
        });
    }

    private void OnGUI()
    {
        // Simple Overlay for Game View testing
        GUILayout.BeginArea(new Rect(10, 10, 250, 200));
        GUILayout.BeginVertical("box");
        GUILayout.Label("<b>Boss Scene Tester</b>");
        
        if (GUILayout.Button("Azhurak's Pyre")) TeleportToScene("Azhurak's Pyre");
        if (GUILayout.Button("Snowfall Cavern")) TeleportToScene("Snowfall Cavern");
        if (GUILayout.Button("Deadwind Cove")) TeleportToScene("Deadwind Cove");
        if (GUILayout.Button("Deepwood Hollow")) TeleportToScene("Deepwood Hollow");
        
        GUILayout.Space(5);
        if (GUILayout.Button("Back to Homebase")) TeleportToScene("Anchorpoint");
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}
