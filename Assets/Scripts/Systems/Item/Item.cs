using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


public interface IItemComponent
{
    public IItemComponent DeepClone();
}

[System.Serializable]
public class Item
{
    [HorizontalGroup("Row1")] // HideLabel, PreviewField(50)
    public ItemData data;

    [HorizontalGroup("Row2"), LabelWidth(60), MinValue(0)]
    public int quantity;

    public ItemModifierMediator ItemModifierMediator { get; private set; }

    [SerializeReference, InlineEditor, ValueDropdown(nameof(GetItemComponentTypes))]
    private List<IItemComponent> components = new List<IItemComponent>();

    // This method will help us recreate the SO_Item reference when loading
    public string dataGUID;

    #region Constructor
    public Item(ItemData baseData, int quantity)
    {
        this.data = baseData;
        this.quantity = quantity;
        this.ItemModifierMediator = new ItemModifierMediator(this);
        // this.dataGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(data));
    }

    public Item(ItemData baseData, int quantity, List<IItemComponent> components): this(baseData, quantity)
    {
        this.components = components;
    }
    #endregion

    #region Item Component Handling

    // TODO: make this private. Other systems should acces Item components via ItemModifierMediator entrypoint.
    public T GetComponent<T>() where T : IItemComponent
    {
        return (T)components.Find(c => c is T);
    }
    public void AddComponent(IItemComponent component)
    {
        components.Add(component);
    }
    public void RemoveComponent<T>() where T : IItemComponent
    {
        components.RemoveAll(c => c is T);
    }
    public bool HasComponent<T>() where T : IItemComponent
    {
        return components.Exists(c => c is T);
    }
    public List<IItemComponent> GetItemComponents()
    {
        return components;
    }
    private static IEnumerable<object> GetItemComponentTypes()
    {
        // For Odin serialization of interfaces.
        yield return new ItemAttackContainerComponent(null);
        yield return new ItemBaseStatComponent();
        yield return new ItemUpgradeComponent();
        yield return new ItemUpgraderComponent();
        yield return new ItemEquipmentComponent(EquipmentType.SWORD);
        yield return new ItemValueComponent();
    }
    #endregion

    #region Utility (Clone, IsEmpty)

    /// <summary>
    /// Creates a deep copy of the item.
    /// </summary>
    /// <returns></returns>
    public Item Clone()
    {
        List<IItemComponent> clonedComponents = new List<IItemComponent>();
        foreach (var component in components)
        {
            clonedComponents.Add(component.DeepClone());
        }

        return new Item(data, quantity, clonedComponents);
    }

    /// <summary>
    /// Checks if data is null or quantity = 0.
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty() => data == null || quantity == 0;
    #endregion

    // TODO: update equals and hash function.
    #region Equals + Hash
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
    #endregion
}
