using UnityEngine.Assertions;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Main centralized script that handles the state of all relevant player components.
/// </summary>
public class PlayerDataPersistenceComponent : MonoBehaviour, IDataPersistence
{
    [Required, SerializeField] private StatComponent stats;

    private void Awake()
    {
        Assert.IsNotNull(stats, "PlayerDataPersistenceComponent is missing reference to StatComponent!");
    }

    public void LoadData(GameData data)
    {
        stats.StatContainer.Health = data.playerCurrentHealth;
    }

    public void SaveData(GameData data)
    {
        data.playerCurrentHealth = stats.StatContainer.Health;
    }
}
