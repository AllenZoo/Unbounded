using UnityEngine;

/// <summary>
/// Extra layer for interactions that can be committed or canceled.
/// </summary>
public interface ICommittableInteraction
{
    void Commit();
    void Cancel();
}
