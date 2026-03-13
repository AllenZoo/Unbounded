using Sirenix.OdinInspector;
using UnityEngine.Assertions;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableObjectDatabase : Singleton<ScriptableObjectDatabase>, IDataPersistence
{
    [Required] public ScriptableObjectDatabaseData Data;

    protected override void Awake()
    {
        base.Awake();
        Assert.IsNotNull(Data);
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
        if (Data == null) return;
        
        foreach (var so in Data.AllScriptableObjects)
        {
            if (so is IDataPersistence persistence)
            {
                persistence.LoadData(data);
            }
        }
    }

    public void SaveData(GameData data)
    {
        if (Data == null) return;

        foreach (var so in Data.AllScriptableObjects)
        {
            if (so is IDataPersistence persistence)
            {
                persistence.SaveData(data);
            }
        }
    }
}