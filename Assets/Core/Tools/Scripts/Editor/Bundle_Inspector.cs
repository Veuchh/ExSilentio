using LW.Data;
using LW.Level;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(RevealableObjectBundle))]
[CanEditMultipleObjects]
public class Bundle_Inspector : Editor
{
    const string DATABASE_PATH = "Assets/Core/Data/ScriptableObjects/HintDatabase.asset";

    static HintDatabase database;

    static HintDatabase Database
    {
        get
        {
            if (database == null)
                database = (HintDatabase)AssetDatabase.LoadAssetAtPath(DATABASE_PATH, typeof(HintDatabase));
            return database;
        }
    }
    public override void OnInspectorGUI()
    {
        RevealableObjectBundle targetBundle = (RevealableObjectBundle)target;
        DrawDefaultInspector();
        EditorGUI.indentLevel++;
        for (int i = 0; i < targetBundle.HintsBase.Count; i++)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("-", GUILayout.Width(50)) 
                || !Database.HintKeys.Contains((targetBundle.HintsBase[i])))
            {
                targetBundle.HintsBase.RemoveAt(i);
                i--;
            }

            targetBundle.HintsBase[i] = Database.HintKeys[EditorGUILayout.Popup(Database.HintKeys.IndexOf(targetBundle.HintsBase[i]),
                                                                          Database.HintKeys.ToArray())];
            GUILayout.EndHorizontal();
        }

        if (GUILayout.Button("+"))
        {
            targetBundle.HintsBase.Add(Database.HintKeys[0]);
        }
        EditorGUI.indentLevel--;

        if (GUILayout.Button("Setup Bundle"))
        {
            targetBundle.SetupElements();
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(targetBundle);
            EditorSceneManager.MarkSceneDirty(targetBundle.gameObject.scene);
        }
    }
}
