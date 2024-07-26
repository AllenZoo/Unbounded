using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Item))]
public class ItemPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Draw default fields
        position.height = EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(position, property.FindPropertyRelative("data"));
        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        EditorGUI.PropertyField(position, property.FindPropertyRelative("quantity"));
        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        // Draw components
        SerializedProperty componentsProperty = property.FindPropertyRelative("serializableComponents");
        EditorGUI.PropertyField(position, componentsProperty, true);
        position.y += EditorGUI.GetPropertyHeight(componentsProperty) + EditorGUIUtility.standardVerticalSpacing;

        // Add component buttons
        if (GUI.Button(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), "Add Attack Component"))
        {
            AddComponent(componentsProperty, SerializableItemComponent.ComponentType.Attack);
        }
        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        if (GUI.Button(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), "Add Upgrade Component"))
        {
            AddComponent(componentsProperty, SerializableItemComponent.ComponentType.Upgrade);
        }
        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        if (GUI.Button(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), "Add Base Stat Component"))
        {
            AddComponent(componentsProperty, SerializableItemComponent.ComponentType.BaseStat);
        }


        EditorGUI.EndProperty();
    }

    private void AddComponent(SerializedProperty componentsProperty, SerializableItemComponent.ComponentType type)
    {
        componentsProperty.InsertArrayElementAtIndex(componentsProperty.arraySize);
        var newElement = componentsProperty.GetArrayElementAtIndex(componentsProperty.arraySize - 1);
        newElement.FindPropertyRelative("type").enumValueIndex = (int)type;

        switch (type)
        {
            case SerializableItemComponent.ComponentType.Attack:
                newElement.FindPropertyRelative("component").managedReferenceValue = new ItemAttackContainerComponent(null);
                break;
            case SerializableItemComponent.ComponentType.Upgrade:
                newElement.FindPropertyRelative("component").managedReferenceValue = new ItemUpgradeComponent();
                break;
            case SerializableItemComponent.ComponentType.BaseStat:
                newElement.FindPropertyRelative("component").managedReferenceValue = new ItemBaseStatComponent();
                break;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight * 2; // baseData and quantity
        height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("serializableComponents"));
        height += EditorGUIUtility.singleLineHeight * 3; // Add component buttons
        height += EditorGUIUtility.singleLineHeight; // paddding
        height += EditorGUIUtility.standardVerticalSpacing * 4;
        return height;
    }
}
