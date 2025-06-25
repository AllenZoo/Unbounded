using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable Object that holds references to all relevant shader materials for use.
/// </summary>
[CreateAssetMenu(fileName = "new shader bank", menuName ="System/Shaders/ShaderBank")]
public class ShaderBank : ScriptableObject
{
    public Material DissolveMaterial;
}
