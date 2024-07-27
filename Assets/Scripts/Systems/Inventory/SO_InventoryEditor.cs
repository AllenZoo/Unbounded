using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SO_Inventory)), CanEditMultipleObjects]
public class SO_InventoryEditor : Editor
{
    private string fileName = "inventory";

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SO_Inventory inventory = (SO_Inventory)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Save/Load Inventory", EditorStyles.boldLabel);

        fileName = EditorGUILayout.TextField("File Name", fileName);

        if (GUILayout.Button("Save Inventory"))
        {
            inventory.SaveInventory(fileName);
        }

        if (GUILayout.Button("Load Inventory"))
        {
            inventory.LoadInventory(fileName);
        }

        // Ensure changes are saved
        if (GUI.changed)
        {
            EditorUtility.SetDirty(inventory);
        }
    }
}