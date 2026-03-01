using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

public class ExtensionsWindow : EditorWindow
{
    [MenuItem("Tools/Scene Tools")]
    public static void ShowWindow()
    {
        var window = GetWindow<ExtensionsWindow>();
        window.titleContent = new GUIContent("Scene Tools");
        window.Show();
    }

    private bool isDomainReloadDisabled => EditorApplication.isCompiling;
    private bool isSceneReloadDisabled => !EditorApplication.isPlaying;

    private void OnGUI()
    {
        Disableable(DomainReloadButton, isDomainReloadDisabled);
        Disableable(SceneReloadButton, isSceneReloadDisabled);
    }

    private void DomainReloadButton()
    {
        if (GUILayout.Button("Reload Domain"))
        {
            EditorUtility.RequestScriptReload();
        }
    }

    private void SceneReloadButton()
    {
        if (GUILayout.Button("Reload Scene"))
        {
            var scene = SceneManager.GetActiveScene();
            if (scene != null)
            {
                var opts = new LoadSceneParameters { };
                #if UNITY_EDITOR
                EditorSceneManager.LoadSceneInPlayMode(scene.path, opts);
                #endif
            }
        }
    }

    private void Disableable(Action renderer, bool disabled)
    {
        EditorGUI.BeginDisabledGroup(disabled);
        renderer();
        EditorGUI.EndDisabledGroup();
    }
}