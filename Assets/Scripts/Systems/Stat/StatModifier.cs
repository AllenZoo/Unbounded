using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatModifier : IDisposable
{
    [SerializeField] public Stat Stat;

    public IOperation operation;
    public bool MarkedForRemoval = false;

    public event Action<StatModifier> OnDispose = delegate { };
    readonly CountdownTimer timer;


    /// <summary>
    /// Creates a new StatModifier with a duration. If duration is negative, the modifier will not expire, but can be disposed on some event.
    /// </summary>
    /// <param name="stat"></param>
    /// <param name="operation"></param>
    /// <param name="duration"></param>
    public StatModifier(Stat stat, IOperation operation, float duration)
    {
        this.Stat = stat;
        this.operation = operation;

        // Don't create timer if duration is negative.
        if (duration < 0) return;

        timer = new CountdownTimer(duration);
        timer.OnTimerStop += () => Dispose();
        timer.Start();
    }

    /// <summary>
    /// Modifies given param query by applying stat modifier onto it.
    /// </summary>
    /// <param name="query"></param>
    public void PerformQuery(StatQuery query)
    {
        if (query.Stat == Stat)
        {
            query.Value = operation.Calculate(query.Value);
        }
    }

    public void Update(float deltaTime) => timer?.Tick(deltaTime);

    public void Dispose()
    {
        OnDispose?.Invoke(this);
    }

    public override string ToString()
    {
        return $"{Stat} {operation.GetValue()}";
    }
}