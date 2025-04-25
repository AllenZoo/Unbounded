using System;

public interface IStatModifierFactory
{
    StatModifier Create(OperationType operatorType, Stat statType, int value, float duration);
}

public static class StatModifierFactory // : IStatModifierFactory
{
    public static StatModifier Create(OperationType operatorType, Stat statType, int value, float duration)
    {
        IOperation strategy = operatorType switch
        {
            OperationType.Add => new AddOperation(value),
            OperationType.Multiply => new MultiplyOperation(value),
            _ => throw new ArgumentOutOfRangeException()
        };

        return new StatModifier(statType, strategy, duration);
    }
}