using System;
using UnityEditor;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public class RequiredComponentsAttribute : PropertyAttribute
{
    public Type[] RequiredComponentTypes { get; private set; }

    public RequiredComponentsAttribute(params Type[] requiredComponentTypes)
    {
        RequiredComponentTypes = requiredComponentTypes;
    }
}

[CustomPropertyDrawer(typeof(RequiredComponentsAttribute))]
public class RequiredComponentsDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        EditorGUI.PropertyField(position, property, label, true);

        var attributeP = (RequiredComponentsAttribute)attribute;

        var gameObject = property.objectReferenceValue as GameObject;

        if (gameObject != null)
        {
            Rect offsetPos = position;
            foreach (var requiredType in attributeP.RequiredComponentTypes)
            {
                if (gameObject.GetComponent(requiredType) == null)
                {
                    offsetPos = new Rect(offsetPos.position - new Vector2(0, -20), offsetPos.size);
                    EditorGUI.HelpBox(offsetPos, $"{requiredType.Name} component is required.", MessageType.Error);
                    // Debug.Log(label.text + property.name + " is missing " + requiredType.Name + " component.");
                    //EditorGUILayout.HelpBox($"{requiredType.Name} component is required.", MessageType.Error);
                }
            }
        }

        EditorGUI.EndProperty();
    }
}
