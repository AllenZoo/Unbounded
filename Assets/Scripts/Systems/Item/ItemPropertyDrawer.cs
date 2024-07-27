using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Item))]
public class ItemPropertyDrawer : PropertyDrawer
{
    private Dictionary<string, bool> showComponentsDict = new Dictionary<string, bool>();
    private Dictionary<string, bool> showButtonsDict = new Dictionary<string, bool>();
    private const float bottomPadding = 10f; // Adjust this value to increase or decrease padding


    private string GetUniqueKey(SerializedProperty property)
    {
        return property.propertyPath;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Drag and drop
        Event evt = Event.current;
        Rect dragRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        if (evt.type == EventType.MouseDrag && dragRect.Contains(evt.mousePosition))
        {
            DragAndDrop.PrepareStartDrag();
            DragAndDrop.objectReferences = new Object[] { property.serializedObject.targetObject };
            DragAndDrop.SetGenericData("ItemProperty", property.propertyPath);
            DragAndDrop.StartDrag("Drag Item");
            evt.Use();
        }


        string uniqueKey = GetUniqueKey(property);

        if (!showComponentsDict.ContainsKey(uniqueKey))
            showComponentsDict[uniqueKey] = false;
        if (!showButtonsDict.ContainsKey(uniqueKey))
            showButtonsDict[uniqueKey] = false;

        // Draw default fields
        position.height = EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(position, property.FindPropertyRelative("data"));
        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        EditorGUI.PropertyField(position, property.FindPropertyRelative("quantity"));
        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        // Draw components
        SerializedProperty componentsProperty = property.FindPropertyRelative("serializableComponents");
        showComponentsDict[uniqueKey] = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), showComponentsDict[uniqueKey], "Components", true);
        position.y += EditorGUIUtility.singleLineHeight;

        if (showComponentsDict[uniqueKey])
        {
            EditorGUI.indentLevel++;
            EditorGUI.PropertyField(position, componentsProperty, true);
            position.y += EditorGUI.GetPropertyHeight(componentsProperty);
            EditorGUI.indentLevel--;
        }

        position.y += EditorGUIUtility.standardVerticalSpacing;

        // Add component buttons
        showButtonsDict[uniqueKey] = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), showButtonsDict[uniqueKey], "Add Components", true);
        position.y += EditorGUIUtility.singleLineHeight;

        if (showButtonsDict[uniqueKey])
        {
            EditorGUI.indentLevel++;
            if (GUI.Button(new Rect(position.x, position.y, position.width - 20, EditorGUIUtility.singleLineHeight), "Add Attack Component"))
            {
                AddComponent(componentsProperty, SerializableItemComponent.ComponentType.Attack);
            }
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            if (GUI.Button(new Rect(position.x, position.y, position.width - 20, EditorGUIUtility.singleLineHeight), "Add Base Stat Component"))
            {
                AddComponent(componentsProperty, SerializableItemComponent.ComponentType.BaseStat);
            }
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            if (GUI.Button(new Rect(position.x, position.y, position.width - 20, EditorGUIUtility.singleLineHeight), "Add Upgrade Component"))
            {
                AddComponent(componentsProperty, SerializableItemComponent.ComponentType.Upgrade);
            }
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            if (GUI.Button(new Rect(position.x, position.y, position.width - 20, EditorGUIUtility.singleLineHeight), "Add Upgrader Component"))
            {
                AddComponent(componentsProperty, SerializableItemComponent.ComponentType.Upgrader);
            }
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.indentLevel--;
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
            case SerializableItemComponent.ComponentType.Upgrader:
                newElement.FindPropertyRelative("component").managedReferenceValue = new ItemUpgraderComponent();
                break;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        string uniqueKey = GetUniqueKey(property);

        float height = EditorGUIUtility.singleLineHeight * 2; // baseData and quantity
        height += EditorGUIUtility.singleLineHeight * 2; // Foldouts

        if (showComponentsDict.ContainsKey(uniqueKey) && showComponentsDict[uniqueKey])
        {
            height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("serializableComponents"));
        }

        if (showButtonsDict.ContainsKey(uniqueKey) && showButtonsDict[uniqueKey])
        {
            height += EditorGUIUtility.singleLineHeight * 4; // Add component buttons
        }

        height += EditorGUIUtility.standardVerticalSpacing * 3;
        height += bottomPadding; // Add bottom padding
        return height;
    }
}
