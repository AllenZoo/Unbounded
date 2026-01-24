using UnityEngine;

// Note: This script is useful for resetting state when domain reloading is disabled in Unity.
public static class PageUIContextBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Reset()
    {
        // Find all PageUIContext assets and reset them
        var contexts = Resources.FindObjectsOfTypeAll<PageUIContext>();
        foreach (var ctx in contexts)
        {
            ctx.ResetContext();
        }
    }
}
