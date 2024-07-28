using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory/Inventory")]
public class SO_Inventory : SerializedScriptableObject
{
    [MinValue(1)]
    public int slots = 9;

    [ListDrawerSettings(ShowIndexLabels = true, OnBeginListElementGUI = "DrawItemHeader")]
    [HideReferenceObjectPicker]
    [PropertySpace(10)]
    public List<Item> items = new List<Item>();

    public event Action OnInventoryDataChange;

    [Button("Add Item"), GUIColor(0.7f, 1f, 0.7f)]
    private void AddItem()
    {
        if (items.Count < slots)
        {
            items.Add(null);
            InvokeOnDataChange();
        }
        else
        {
            Debug.LogWarning("Inventory is full. Cannot add more items.");
        }
    }

    [Button("Remove Last Item"), GUIColor(1f, 0.7f, 0.7f)]
    private void RemoveLastItem()
    {
        if (items.Count > 0)
        {
            items.RemoveAt(items.Count - 1);
            InvokeOnDataChange();
        }
        else
        {
            Debug.LogWarning("Inventory is empty. Cannot remove items.");
        }
    }

    private void DrawItemHeader(int index)
    {
        GUILayout.Space(5);
        GUILayout.Label($"Item {index + 1}", EditorStyles.boldLabel);
    }

    [OnInspectorGUI]
    private void OnInspectorGUI()
    {
        if (items.Count != slots)
        {
            EditorGUILayout.HelpBox($"Item count ({items.Count}) does not match slot count ({slots}). Click 'Adjust Items' to fix.", MessageType.Warning);
            if (GUILayout.Button("Adjust Items"))
            {
                AdjustItemsToSlots();
            }
        }
    }

    private void AdjustItemsToSlots()
    {
        if (items.Count > slots)
        {
            items.RemoveRange(slots, items.Count - slots);
        }
        else if (items.Count < slots)
        {
            int difference = slots - items.Count;
            for (int i = 0; i < difference; i++)
            {
                items.Add(null);
            }
        }
        InvokeOnDataChange();
    }

    public void InvokeOnDataChange()
    {
        EventBus<OnInventoryModifiedEvent>.Call(new OnInventoryModifiedEvent());
        OnInventoryDataChange?.Invoke();
    }

    [Button("Set Item")]
    public void Set(int index, Item item)
    {
        if (index >= 0 && index < items.Count)
        {
            items[index] = item;
            InvokeOnDataChange();
        }
        else
        {
            Debug.LogError($"Invalid index: {index}. Must be between 0 and {items.Count - 1}.");
        }
    }

    [Button("Check If Empty")]
    public bool IsEmpty()
    {
        foreach (Item item in items)
        {
            if (item != null && !item.IsEmpty())
            {
                return false;
            }
        }
        return true;
    }

    [System.Serializable]
    private class SerializableInventory
    {
        public int slots;
        public List<Item> items;
    }

    [Button("Save Inventory")]
    public void SaveInventory(string fileName)
    {
        SerializableInventory serializableInventory = new SerializableInventory
        {
            slots = this.slots,
            items = this.items
        };
        string json = JsonUtility.ToJson(serializableInventory, true);
        File.WriteAllText(Application.persistentDataPath + "/" + fileName + ".json", json);
        Debug.Log("Inventory saved to " + Application.persistentDataPath + "/" + fileName + ".json");
    }

    [Button("Load Inventory")]
    public void LoadInventory(string fileName)
    {
        string path = Application.persistentDataPath + "/" + fileName + ".json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SerializableInventory serializableInventory = JsonUtility.FromJson<SerializableInventory>(json);
            this.slots = serializableInventory.slots;
            this.items = serializableInventory.items;
            // Restore SO_Item references
            for (int i = 0; i < this.items.Count; i++)
            {
                if (this.items[i] != null && !string.IsNullOrEmpty(this.items[i].dataGUID))
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(this.items[i].dataGUID);
                    this.items[i].data = AssetDatabase.LoadAssetAtPath<SO_Item>(assetPath);
                    if (this.items[i].data == null)
                    {
                        Debug.LogWarning($"Failed to load SO_Item for item at index {i}");
                    }
                }
            }
            AdjustItemsToSlots();
            InvokeOnDataChange();
            Debug.Log("Inventory loaded from " + path);
        }
        else
        {
            Debug.LogWarning("Save file not found: " + path);
        }
    }

    [Button("Clear Inventory")]
    public void ClearInventory()
    {
        items.Clear();
        AdjustItemsToSlots();
    }
}