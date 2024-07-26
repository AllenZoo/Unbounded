using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public interface IItemComponent
{
    
}

[System.Serializable]
public class Item
{
    [SerializeField] public SO_Item data;
    [SerializeField] public int quantity;
    [SerializeField]
    private List<SerializableItemComponent> serializableComponents = new List<SerializableItemComponent>();

    public List<IItemComponent> Components => serializableComponents.Select(sc => sc.component).ToList();


    public Item(SO_Item baseData, int quantity)
    {
        this.data = baseData;
        this.quantity = quantity;
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
