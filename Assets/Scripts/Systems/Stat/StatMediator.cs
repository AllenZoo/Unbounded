using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class that handles the final stat calculation after taking in modifiers.
/// </summary>
public class StatMediator
{
    private LocalEventHandler localEventHander;
    private StatComponent stat;
    private List<StatModifier> modifiers = new List<StatModifier>();
    private Dictionary<Stat, IEnumerable<StatModifier>> modifiersCache = new Dictionary<Stat, IEnumerable<StatModifier>>();
    private IStatModifierApplicationOrder order = new NormalStatModifierOrder();

    public StatMediator(LocalEventHandler localEventHander, StatComponent stat)
    {
        this.localEventHander = localEventHander;
        this.stat = stat;
    }

    /// <summary>
    /// Util for calculating aggregate value of a stat after applying all modifiers.
    /// </summary>
    /// <param name="stat"></param>
    /// <param name="query"></param>
    /// <param name="orderStrategy"></param>
    public static void CalculateFinalStat(List<StatModifier> stat, StatQuery query, IStatModifierApplicationOrder orderStrategy)
    {
        List<StatModifier> relModifiers = stat.FindAll(mod => mod.Stat == query.Stat);
        query.Value = orderStrategy.Apply(relModifiers, query.Value);
    }

    /// <summary>
    /// Util for calculating aggregate value of a stat after applying all modifiers.
    /// </summary>
    /// <param name="stat"></param>
    /// <param name="query"></param>
    /// <param name="orderStrategy"></param>
    public static void CalculateFinalStat(List<IStatModifierContainer> stat, StatQuery query, IStatModifierApplicationOrder orderStrategy)
    {
        // Convert List<IStatModifierContainer> to List<StatModifier>
        List<StatModifier> relModifiers = stat.Select(container => container.GetModifier()).ToList();
        CalculateFinalStat(relModifiers, query, orderStrategy);
    }

    public void CalculateFinalStat(StatQuery query)
    {
        if (!modifiersCache.ContainsKey(query.Stat))
        {
            modifiersCache.Add(query.Stat, modifiers.FindAll(mod => mod.Stat == query.Stat));
            
        }

        query.Value = order.Apply(modifiersCache[query.Stat], query.Value);
    }

    public void AddModifier(StatModifier modifier)
    {
        modifiers.Add(modifier);
        modifier.MarkedForRemoval = false;

        InvalidateCache(modifier.Stat);

        modifier.OnDispose += (modifier) => InvalidateCache(modifier.Stat);
        modifier.OnDispose += (modifier) => modifiers.Remove(modifier);

        localEventHander.Call(new OnStatChangeEvent { statComponent = stat, statModifier = modifier });
    }
    public void RemoveModifier(StatModifier modifier)
    {
        modifiers.Remove(modifier);
        InvalidateCache(modifier.Stat);
    }
    private void InvalidateCache(Stat stat)
    {
        modifiersCache.Remove(stat);
    }
    public void Update(float deltaTime)
    {
        foreach (var modifier in modifiers)
        {
            modifier.Update(deltaTime);
        }

        foreach (var modifier in modifiers.Where(modifier => modifier.MarkedForRemoval).ToList())
        {
            modifier.Dispose();
        }
    }
}

/// <summary>
/// Class that gets passed through StatMediator to each StatModifier to calculate the final stat.
/// </summary>
public class StatQuery
{
    public Stat Stat;
    public float Value;

    public StatQuery(Stat stat, float value)
    {
        this.Stat = stat;
        this.Value = value;
    }

    public override string ToString()
    {
        return $"{Stat} with base: {Value}";
    }
}
