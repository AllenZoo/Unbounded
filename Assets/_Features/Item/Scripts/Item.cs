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
    public virtual void Load(Item item) { }
}

[System.Serializable]
public class Item: IModel
{
    [HorizontalGroup("Row1")] // HideLabel, PreviewField(50)
                              
    public ItemData Data => data;

    [UnityEngine.SerializeField]
    private ItemData data;

    [OdinSerialize, ShowInInspector, ReadOnly] private string dataGUID;

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
                Init();
            }

            return itemModifierMediator;
        }
        private set => itemModifierMediator = value;
    }
    private ItemModifierMediator itemModifierMediator;

    // TODO: init these properties through scriptable object.
    [SerializeReference, InlineEditor, ValueDropdown(nameof(GetItemComponentTypes))]
    public List<IItemComponent> components = new List<IItemComponent>();

    private bool isInitialized = false;

    #region Constructor
    public Item()
    {
        // For creating empty Item.
    }

    public void Init(ItemData data, int quantity)
    {
        if (data == null) return;

        this.data = data;
        this.components = data.initialComponents.Select(c => c.DeepClone()).ToList();
        this.quantity = quantity;

        Init();
    }

    // Should be run once per unique Item object. 
    private void Init()
    {
        if (isInitialized) return;

        if (IsEmpty()) return;

        this.itemModifierMediator = new ItemModifierMediator(this);


        if (Data != null)
        {
            this.dataGUID = Data.ID;
        }
        
        foreach (var component in components)
        {
            component.Init();
        }

        isInitialized = true;
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
    /// 
    /// Item is already properly initialized, so the clone will have its ItemModifierMediator and components ready to use.
    /// </summary>
    /// <returns></returns>
    public Item DeepClone()
    {
        Item item = new Item();
        item.Init(Data, quantity);
        return item;
    }

    /// <summary>
    /// Checks if data is null or quantity = 0.
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty() => Data == null || quantity == 0;
    #endregion

    #region DataPersistence

    public void Load(Item item)
    {
        if (item == null) return;
        this.quantity = item.quantity;
        if (item.dataGUID != null)
        {
            // Load the ItemData from Database
            data = ScriptableObjectDatabase.Instance.Data.Get<ItemData>(item.dataGUID);
        }

        if (components == null) return;

        foreach (var component in components)
        {
            component.Load(item);
        }
    }

    public void Save()
    {

    }

    #endregion

    #region Equals + Hash
    public override bool Equals(object obj)
    {
        if (obj is not Item other)
            return false;

        if (!string.Equals(dataGUID, other.dataGUID, StringComparison.Ordinal))
            return false;

        if (quantity != other.quantity)
            return false;

        if (components == null && other.components == null) return true;
        if (components == null || other.components == null) return false;
        if (components.Count != other.components.Count) return false;

        // Multiset comparison: group by component, compare counts
        var thisGroups = components.GroupBy(c => c).ToDictionary(g => g.Key, g => g.Count());
        var otherGroups = other.components.GroupBy(c => c).ToDictionary(g => g.Key, g => g.Count());

        return thisGroups.Count == otherGroups.Count &&
               thisGroups.All(kvp => otherGroups.TryGetValue(kvp.Key, out var count) && count == kvp.Value);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 31 + (dataGUID != null ? dataGUID.GetHashCode() : 0);
            hash = hash * 31 + quantity.GetHashCode();

            if (components != null && components.Count > 0)
            {
                // Aggregate component hashes in a commutative way (order doesn�t matter)
                int compHash = 0;
                foreach (var comp in components)
                {
                    compHash += comp?.GetHashCode() ?? 0; // addition is commutative
                }

                hash = hash * 31 + compHash;
            }

            return hash;
        }
    }


    #endregion
}
