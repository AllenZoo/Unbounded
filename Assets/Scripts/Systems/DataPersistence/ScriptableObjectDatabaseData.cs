using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public interface IIdentifiableSO
{
    string ID { get; }
}

[CreateAssetMenu(menuName = "System/Database/ScriptableObject Database")]
public class ScriptableObjectDatabaseData : SerializedScriptableObject
{
    [TableList(AlwaysExpanded = true)]
    [SerializeField]
    private List<ScriptableObject> allScriptableObjects = new();

    private Dictionary<string, ScriptableObject> guidToSO;

    public T Get<T>(string id) where T : ScriptableObject, IIdentifiableSO
    {
        Debug.Log($"Trying to get object with id: {id}");

        if (guidToSO == null) BuildLookup();

        return guidToSO.TryGetValue(id, out var so) ? so as T : null;
    }

    private void BuildLookup()
    {
        guidToSO = new Dictionary<string, ScriptableObject>();

        foreach (var so in allScriptableObjects)
        {
            if (so is IIdentifiableSO identifiable && !string.IsNullOrEmpty(identifiable.ID))
            {
                guidToSO[identifiable.ID] = so;
            }
        }
    }

#if UNITY_EDITOR

    [Button("Refresh From Project Assets")]
    private void RefreshDatabase()
    {
        allScriptableObjects.Clear();

        var soGuids = UnityEditor.AssetDatabase.FindAssets("t:ScriptableObject");

        foreach (var guid in soGuids)
        {
            var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            var so = UnityEditor.AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);

            if (so is IIdentifiableSO)
            {
                allScriptableObjects.Add(so);
            }
        }

        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();
    }

#endif
}
