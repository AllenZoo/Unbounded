using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnRates", menuName = "ScriptableObjs/SpawnRates", order = 1)]
public class SO_SpawnRates : ScriptableObject
{
    public List<SpawnRate> spawnRates = new List<SpawnRate>();
}

[System.Serializable]
public class SpawnRates
{
    public SO_SpawnRates data;
}

[System.Serializable]
public class SpawnRate
{
    // TODO: add assertions to check pfb has Spawnable component
    // [GameObjectWithSpawnable]
    public GameObject prefab;
    public float minSpawn;
    // public float maxSpawn;

    /// <summary>
    /// Measured in scale of 1. Rate is relative to other spawn rates.
    /// </summary>
    [Tooltip("Measured in scale of 1. Rate is relative to other spawn rates.")]
    public float spawnRate;
}

// Custom property drawer to enforce the requirement
[CustomPropertyDrawer(typeof(GameObjectWithSpawnableAttribute))]
public class GameObjectWithSpawnableDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty gameObjectProperty = property.FindPropertyRelative("prefab");
        GameObject gameObject = null;
        if (gameObjectProperty != null)
        {
            gameObject = (GameObject) gameObjectProperty.objectReferenceValue;
        }


        if (gameObject != null && (
            gameObject.GetComponent<Spawnable>() == null ||
            gameObject.GetComponentInChildren<Spawnable>() == null))
        {
            EditorGUI.HelpBox(position, "Prefab must contain a 'Spawnable' component.", MessageType.Error);
        }
        else
        {
            EditorGUI.PropertyField(position, gameObjectProperty, label);
        }

        EditorGUI.EndProperty();
    }
}

// Custom attribute to mark GameObject fields that should contain a Spawnable component
public class GameObjectWithSpawnableAttribute : PropertyAttribute { }
