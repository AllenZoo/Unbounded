/// <summary>
/// Interface for all upgrade modifiers related to upgrade card data.
/// </summary>
public interface IUpgradeModifier
{
    void ApplyModifier(); // add context to apply to. eg. ApplyModifier(PlayerContext context) or maybe ApplyModifier(IModifiable modifiable) ??
}


