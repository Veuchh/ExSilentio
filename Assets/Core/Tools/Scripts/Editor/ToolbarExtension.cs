using LW.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityToolbarExtender;

[InitializeOnLoad]
public class ToolbarExtension
{
    const string SCENE_REFERENCE_SO_PATH = "Assets/Core/Tools/ScriptableObjects/DebugSceneReferences.asset";

    static ToolbarExtension()
    {
        ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
    }

    static void OnToolbarGUI()
    {
        GUILayout.FlexibleSpace();

        if (GUILayout.Button(new GUIContent("Load Debug Scene", "Add debug scene to test your scene")))
        {
            SceneReferences so = (SceneReferences)AssetDatabase.LoadAssetAtPath(SCENE_REFERENCE_SO_PATH, typeof(SceneReferences));

            foreach (int sceneIndex in so.AdditiveScenes)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(sceneIndex);
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            }

        }
    }
}