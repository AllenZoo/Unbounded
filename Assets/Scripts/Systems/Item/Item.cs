using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


public interface IItemComponent
{
    
}

[System.Serializable]
public class Item
{
    [SerializeField] public SO_Item data;
    [SerializeField] public int quantity;
    [SerializeField] public List<SerializableItemComponent> serializableComponents = new List<SerializableItemComponent>();
    private List<IItemComponent> Components => serializableComponents.Select(sc => sc.component).ToList();


    // This method will help us recreate the SO_Item reference when loading
    public string dataGUID;

    public Item(SO_Item baseData, int quantity)
    {
        this.data = baseData;
        this.quantity = quantity;
        this.dataGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(data));
    }

    public Item(SO_Item data, int quantity, List<SerializableItemComponent> serializableComponents) : this(data, quantity)
    {
        this.serializableComponents = serializableComponents;
    }

    public T GetComponent<T>() where T : IItemComponent
    {
        return (T)Components.Find(c => c is T);
    }

    public void AddComponent(IItemComponent component)
    {
        var serializableComponent = new SerializableItemComponent();
        serializableComponent.SetComponent(component);
        serializableComponents.Add(serializableComponent);
    }

    public bool HasComponent<T>() where T : IItemComponent
    {
        return Components.Exists(c => c is T);
    }

    public List<IItemComponent> GetItemComponents()
    {
        return Components;
    }

    public Item Clone()
    {
        // TODO: might need to deep copy the components.
        return new Item(data, quantity, serializableComponents);
    }

    public bool IsEmpty() => data == null || quantity == 0;

    // TODO: update equals and hash function.
    /// <summary>
    /// Override Equals method.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        Item other = obj as Item;
        return data.Equals(other.data) && quantity == other.quantity;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(data.GetHashCode(), quantity);
    }
}
