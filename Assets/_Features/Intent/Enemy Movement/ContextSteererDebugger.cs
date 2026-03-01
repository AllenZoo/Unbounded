using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

[RequireComponent(typeof(ContextSteerer))]
public class ContextSteererDebugger : MonoBehaviour
{
    private ContextSteerer steerer;

    private static readonly string[] DirectionLabels = new string[]
    {
        "Up", "Up-Right", "Right", "Down-Right", "Down", "Down-Left", "Left", "Up-Left",
        "Up-Right-Int", "Right-Up-Int", "Right-Down-Int", "Down-Right-Int",
        "Down-Left-Int", "Left-Down-Int", "Left-Up-Int", "Up-Left-Int"
    };

    private void Awake()
    {
        steerer = GetComponent<ContextSteerer>();
    }

    [TabGroup("Weights")]
    [ShowInInspector, ReadOnly]
    [TableList]
    public List<WeightInfo> WeightsInfo
    {
        get
        {
            if (steerer == null) steerer = GetComponent<ContextSteerer>();
            if (steerer == null || steerer.Directions == null || steerer.TargetDirWeights == null) return null;

            var list = new List<WeightInfo>();
            for (int i = 0; i < steerer.Directions.Length; i++)
            {
                list.Add(new WeightInfo
                {
                    Index = i,
                    Label = i < DirectionLabels.Length ? DirectionLabels[i] : "Unknown",
                    Direction = steerer.Directions[i],
                    TargetWeight = (float)steerer.TargetDirWeights[i],
                    DangerWeight = (float)steerer.DangerDirWeights[i],
                    FinalWeight = Mathf.Clamp((float)(steerer.TargetDirWeights[i] - steerer.DangerDirWeights[i]), 0, 1)
                });
            }
            return list;
        }
    }

    public struct WeightInfo
    {
        public int Index;
        public string Label;
        public Vector2 Direction;
        public float TargetWeight;
        public float DangerWeight;
        public float FinalWeight;
    }


    [Button]
    private void LogCurrentWeights()
    {
        if (steerer == null) steerer = GetComponent<ContextSteerer>();
        if (steerer == null) return;

        var targetWeights = steerer.TargetDirWeights;
        var dangerWeights = steerer.DangerDirWeights;

        Debug.Log("--- Context Steerer Weights ---");
        for (int i = 0; i < steerer.Directions.Length; i++)
        {
            string label = i < DirectionLabels.Length ? DirectionLabels[i] : "Unknown";
            float t = (float)targetWeights[i];
            float d = (float)dangerWeights[i];
            float f = Mathf.Clamp(t - d, 0, 1);
            Debug.Log($"Dir {i} [{label}] ({steerer.Directions[i]}): Target={t:F2}, Danger={d:F2}, Final={f:F2}");
        }
    }
}
