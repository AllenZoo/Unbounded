using Sirenix.OdinInspector;
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
    [HorizontalGroup("Row1")]
    // [HideLabel]
    // [PreviewField(50)]
    public SO_Item data;

    [HorizontalGroup("Row2")]
    [LabelWidth(60)]
    [MinValue(0)]
    public int quantity;

    [PropertySpace(10)]
    [ListDrawerSettings(ShowIndexLabels = true, AddCopiesLastElement = true)]
    [HideReferenceObjectPicker]
    public List<SerializableItemComponent> serializableComponents = new List<SerializableItemComponent>();

    private List<IItemComponent> Components => serializableComponents.Select(sc => sc.component).ToList();

    #region Editor Buttons
    [Button("Add Attack Component")]
    private void AddAttackComponent()
    {
        serializableComponents.Add(new SerializableItemComponent(SerializableItemComponent.ComponentType.Attack, new ItemAttackContainerComponent(null)));
    }

    [Button("Add Base Stat Component")]
    private void AddBaseStatComponent()
    {
        serializableComponents.Add(new SerializableItemComponent(SerializableItemComponent.ComponentType.BaseStat, new ItemBaseStatComponent()));
    }

    [Button("Add Upgrade Component")]
    private void AddUpgradeComponent()
    {
        serializableComponents.Add(new SerializableItemComponent(SerializableItemComponent.ComponentType.Upgrade, new ItemUpgradeComponent()));
    }

    [Button("Add Upgrader Component")]
    private void AddUpgraderComponent()
    {
        serializableComponents.Add(new SerializableItemComponent(SerializableItemComponent.ComponentType.Upgrader, new ItemUpgraderComponent()));
    }
    #endregion


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

    /**
     * Creates a deep copy of the item.
     **/
    public Item Clone()
    {
        List<SerializableItemComponent> clonedComponents = new List<SerializableItemComponent>();
        foreach (var component in serializableComponents)
        {
            clonedComponents.Add(component.DeepCopy());
        }

        return new Item(data, quantity, clonedComponents);
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
