// TODO: eventually reimplement this.

//using UnityEngine;
//using UnityEditor;

//[CustomEditor(typeof(ItemDatabase))]
//public class ItemDatabaseEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        DrawDefaultInspector();

//        ItemDatabase itemDB = (ItemDatabase)target;

//        if (GUILayout.Button("Save Database"))
//        {
//            itemDB.SaveDatabase();
//        }

//        if (GUILayout.Button("Load Database"))
//        {
//            itemDB.LoadDatabase();
//            EditorUtility.SetDirty(itemDB);
//        }
//    }
//}