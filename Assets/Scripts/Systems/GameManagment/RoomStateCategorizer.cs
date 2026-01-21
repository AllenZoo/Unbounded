using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomStateCategorizer : Singleton<RoomStateCategorizer>
{
    [SerializeField] private Dictionary<SceneField, RoomState> sceneRoomStateMapping = new Dictionary<SceneField, RoomState>();

    public RoomState GetRoomStateForActiveScene() {
        SceneField activeScene = new SceneField(SceneManager.GetActiveScene().name);
        return GetRoomStateForScene(activeScene);
    }

    public RoomState GetRoomStateForScene(SceneField scene)
    {
        if (sceneRoomStateMapping.TryGetValue(scene, out RoomState roomState))
        {
            return roomState;
        }
        else
        {
            Debug.LogWarning($"[RoomStateCategorizer] Scene {scene} not found in mapping. Defaulting to Exploration.");
            return RoomState.Null;
        }
    }
}
