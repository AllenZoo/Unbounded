using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


public interface IItemComponent
{
    public IItemComponent DeepClone();
    public virtual void Init() { }
}

[System.Serializable]
public class Item
{
    [HorizontalGroup("Row1")] // HideLabel, PreviewField(50)
    public ItemData data;
    [OdinSerialize, ReadOnly] private string dataGUID;

    [HorizontalGroup("Row2"), LabelWidth(60), MinValue(0)]
    public int quantity;

    [JsonIgnore]
    public ItemModifierMediator ItemModifierMediator
    {
        get
        {
            if (itemModifierMediator == null)
            {
                Debug.LogWarning($"[Item] ItemModifierMediator was accessed while still null. " +
                                 $"Ensure Init() was called before accessing this property.");
            }

            return itemModifierMediator;
        }
        private set => itemModifierMediator = value;
    }
    private ItemModifierMediator itemModifierMediator;

    [SerializeReference, InlineEditor, ValueDropdown(nameof(GetItemComponentTypes))]
    public List<IItemComponent> components = new List<IItemComponent>();


    #region Constructor
    public Item(ItemData baseData, int quantity)
    {
        this.data = baseData;
        this.quantity = quantity;
        this.itemModifierMediator = new ItemModifierMediator(this);
        this.dataGUID = data.ID;
    }

    public Item(ItemData baseData, int quantity, List<IItemComponent> components): this(baseData, quantity)
    {
        this.components = components;
        this.dataGUID = data.ID;
    }

    public void Init()
    {
        this.itemModifierMediator = new ItemModifierMediator(this);
        this.dataGUID = data.ID;
        foreach (var component in components)
        {
            component.Init();
        }
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
        // yield return new ItemUpgraderComponent();
        yield return new ItemEquipmentComponent(EquipmentType.SWORD);
        //yield return new ItemValueComponent();
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

    #region DataPersistence

    public void Load()
    {
        // Load the ItemData from Database
        data = ScriptableObjectDatabase.Instance.Data.Get<ItemData>(dataGUID);
    }

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
