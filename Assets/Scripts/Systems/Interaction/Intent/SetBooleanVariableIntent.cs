using UnityEngine;

public class SetBooleanVariableIntent : ICommittableInteraction
{
    readonly private ScriptableObjectBoolean boolVariable;
    readonly private bool valueToSet;

    public SetBooleanVariableIntent(ScriptableObjectBoolean variable, bool valueToSet)
    {
        boolVariable = variable;
        this.valueToSet = valueToSet;
    }

    public void Commit()
    {
        boolVariable.Set(valueToSet);
    }

    public void Cancel()
    {
        // Intentionally empty – no side effects
    }
}
