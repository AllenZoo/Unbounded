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

    // Optional method to reset data to default values when starting a new game or resetting progress.
    // For values that should be reset to default when starting a new game, implement this method to set those values accordingly (e.g. plaerHeatlh, roundNumber).
    // For values that should be preserved across game sessions, do not implement this method or leave it empty (e.g. settings).
    void ResetData();
}
