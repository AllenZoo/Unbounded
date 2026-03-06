using System;
using UnityEngine;

public interface IObjectiveCondition
{
    bool IsMet();
    event Action OnStateChanged;
    void Initialize(Objective owner);
    void Cleanup();
}
