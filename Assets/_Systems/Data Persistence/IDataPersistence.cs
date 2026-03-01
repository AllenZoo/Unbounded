using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for all the scripts that require loading and saving of data.
/// </summary>
public interface IDataPersistence
{
    void LoadData(GameData data);
    void SaveData(GameData data);
}
