using LW.Level;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityToolbarExtender;



[InitializeOnLoad]
public class ToolbarExtension
{
    const string SCENE_REFERENCE_SO_PATH = "Assets/Core/Tools/ScriptableObjects/DebugSceneReferences.asset";
    const string LEVEL_HANDLER_PREFAB_PATH = "Assets/Core/Level/Prefabs/RevealableObjectHandler.prefab";

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
        if (GUILayout.Button(new GUIContent("Make scene game ready")))
        {
            GameObject handler = Object.FindObjectOfType<RevealableObjectHandler>()?.gameObject;
            if (handler == null)
            {
                handler = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(LEVEL_HANDLER_PREFAB_PATH));
            }

            Selection.activeGameObject = handler;

        }
    }
}