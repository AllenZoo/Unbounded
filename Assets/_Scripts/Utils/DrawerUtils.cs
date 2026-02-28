#if UNITY_EDITOR
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public static class DrawerUtils
{
    public static IEnumerable<ValueDropdownItem<int>> GetSortingLayers()
    {
        foreach (var layer in SortingLayer.layers)
        {
            yield return new ValueDropdownItem<int>(layer.name, layer.id);
        }
    }
}
#endif
