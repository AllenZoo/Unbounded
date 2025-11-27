using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class AttackIndicatorData : ScriptableObject
{
    [Tooltip("GameObject representation of attack indicator.")]
    [Required, JsonIgnore] public GameObject attackIndicatorPfb;

    [Tooltip("The time it takes for the attack indicator to fully transition in or out. (from start to end fill colour")]
    public float transitionTime = 0.5f;

    public Color startFillColour = Color.red;
    public Color endFillColour = new Color(1f, 0f, 0f, 0.5f);

}
