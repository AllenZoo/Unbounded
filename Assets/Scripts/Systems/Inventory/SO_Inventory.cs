using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory/Inventory")]
public class SO_Inventory : ScriptableObject
{
    public int slots = 9;
    public List<Item> items = new List<Item>();

    public event Action OnInventoryDataChange;

    private void OnValidate()
    {
        // Match size of items to slots
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

        // Filter out any "missing" item slots
        //foreach (var item in items)
        //{
        //    if (item.data == null)
        //    {
        //        item.data = null;
        //    }
        //}
    }

    public void InvokeOnDataChange()
    {
        EventBus<OnInventoryModifiedEvent>.Call(new OnInventoryModifiedEvent());
        OnInventoryDataChange?.Invoke();
    }

    public void Set(int index, Item item)
    {
        items[index] = item;
        InvokeOnDataChange();
    }

    /// <summary>
    /// Returns the number of items (non null) in the inventory.
    /// </summary>
    /// <returns></returns>
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

            OnValidate();
            InvokeOnDataChange();
            Debug.Log("Inventory loaded from " + path);
        }
        else
        {
            Debug.LogWarning("Save file not found: " + path);
        }
    }
}
