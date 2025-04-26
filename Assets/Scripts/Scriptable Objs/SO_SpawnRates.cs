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
    [GameObjectWithSpawnable]
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

        GameObject gameObject = (GameObject) property.objectReferenceValue;

        float x_padding = 80;
        float y_padding = -20;

        if (gameObject != null &&
            gameObject.GetComponentOrInChildren<Spawnable>() == null)
        {
            Vector2 offset = new Vector2(x_padding, y_padding);
            Rect errBoxPos = new Rect(position.position + offset, position.size - new Vector2(x_padding, 0));
            EditorGUI.HelpBox(errBoxPos, "Prefab must contain a 'Spawnable' component.", MessageType.Error);
            Debug.LogError("SpawnRate Prefab in \'" + gameObject.name + " GameObject\' does not contain a 'Spawnable' component.");
        }

        EditorGUI.PropertyField(position, property, label);
        EditorGUI.EndProperty();
    }
}

// Custom attribute to mark GameObject fields that should contain a Spawnable component
public class GameObjectWithSpawnableAttribute : PropertyAttribute { }
