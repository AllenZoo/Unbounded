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

    
    private void OnEnable()
    {
        if (DataPersistenceHandler.Instance != null)
            DataPersistenceHandler.Instance.Register(this);
    }

    private void OnDisable()
    {
        if (DataPersistenceHandler.Instance != null)
            DataPersistenceHandler.Instance.Unregister(this);
    }
public void LoadData(GameData data)
    {
        stats.StatContainer.Health = data.playerCurrentHealth;
        stats.StatContainer.Gold = data.playerGold;
    }

    public void SaveData(GameData data)
    {
        data.playerCurrentHealth = stats.StatContainer.Health;
        data.playerGold = stats.StatContainer.Gold;
    }
}
