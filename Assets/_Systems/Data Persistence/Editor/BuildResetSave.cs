using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.IO;

public class BuildResetSave : IPreprocessBuildWithReport
{
    // High priority to run early in the build process
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        // Based on DataPersistenceHandler, the file name is "treasure"
        // The user referred to it as "treasure.json", so we'll check for both.
        string[] fileNamesToSearch = { "treasure", "treasure.json" };
        string persistentPath = Application.persistentDataPath;

        if (Directory.Exists(persistentPath))
        {
            foreach (string baseName in fileNamesToSearch)
            {
                // Search for the save file and backups in all profile subdirectories
                string[] files = Directory.GetFiles(persistentPath, baseName, SearchOption.AllDirectories);
                string[] backups = Directory.GetFiles(persistentPath, baseName + ".bak", SearchOption.AllDirectories);

                foreach (string file in files)
                {
                    try {
                        File.Delete(file);
                        Debug.Log($"[BuildResetSave] Deleted save file before build: {file}");
                    } catch (System.Exception e) {
                        Debug.LogError($"[BuildResetSave] Failed to delete {file}: {e.Message}");
                    }
                }

                foreach (string backup in backups)
                {
                    try {
                        File.Delete(backup);
                        Debug.Log($"[BuildResetSave] Deleted backup file before build: {backup}");
                    } catch (System.Exception e) {
                        Debug.LogError($"[BuildResetSave] Failed to delete {backup}: {e.Message}");
                    }
                }
            }
        }
    }
}
