using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using System.Linq;

public class ItemDatabase : MonoBehaviour
{
    public List<Item> items = new List<Item>();
    private string savePath;

    [System.Serializable]
    private class SerializableItem
    {
        public string itemDataPath;
        public int quantity;
        public List<SerializableItemComponent> components;
    }

    [System.Serializable]
    private class SerializableItemList
    {
        public List<SerializableItem> items = new List<SerializableItem>();
    }

    void Awake()
    {
        InitializeSavePath();
    }

    private void InitializeSavePath()
    {
        if (string.IsNullOrEmpty(savePath))
        {
            savePath = Path.Combine(Application.persistentDataPath, "itemDatabase.json");
        }
    }

    public void SaveDatabase()
    {
        InitializeSavePath();

        SerializableItemList serializableList = new SerializableItemList();
        foreach (Item item in items)
        {
            if (item != null && item.data != null)
            {
                SerializableItem serializableItem = new SerializableItem
                {
                    itemDataPath = AssetDatabase.GetAssetPath(item.data),
                    quantity = item.quantity,
                    components = item.serializableComponents
                };
                serializableList.items.Add(serializableItem);
            }
        }

        string json = JsonUtility.ToJson(serializableList, true);
        File.WriteAllText(savePath, json);

        Debug.Log("Database saved to " + savePath);
    }

    public void LoadDatabase()
    {
        InitializeSavePath();

        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            SerializableItemList serializableList = JsonUtility.FromJson<SerializableItemList>(json);

            items.Clear();
            foreach (SerializableItem serializableItem in serializableList.items)
            {
                ItemData itemData = AssetDatabase.LoadAssetAtPath<ItemData>(serializableItem.itemDataPath);
                if (itemData != null)
                {
                    Item item = new Item(itemData, serializableItem.quantity);
                    item.serializableComponents = serializableItem.components;
                    items.Add(item);
                }
            }

            Debug.Log("Database loaded from " + savePath);
        }
        else
        {
            Debug.LogWarning("Save file not found in " + savePath);
        }
    }
}