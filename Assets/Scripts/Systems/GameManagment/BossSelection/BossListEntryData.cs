using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "new BossListEntryData", menuName = "System/GameManagement/BossSelection/BossListEntryData", order = 1)]
public class BossListEntryData : SerializedScriptableObject
{
    [Required] public string bossName;
    [Required] public SceneField bossScene;
    [Required] public GameObject boss;
}
