using LW.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

namespace LW.Editor.Tools.WordDatabaseTool
{
    public class WordDatabaseTool : EditorWindow
    {
        const string DATABASE_PATH = "Assets/Core/Data/ScriptableObjects/WordDatabase.asset";
        const string STRING_TABLE_NAME = "TextTable";

        static EditorWindow window;

        WordDatabase database;
        Vector2 scrollPositon;

        [MenuItem("Window/My Window")]
        public static void ShowWindow()
        {
            window = GetWindow(typeof(WordDatabaseTool));
        }

        void OnGUI()
        {
            scrollPositon = EditorGUILayout.BeginScrollView(scrollPositon);

            var collection = UnityEditor.Localization.LocalizationEditorSettings.GetStringTableCollection(STRING_TABLE_NAME);
            foreach (WordDatabaseEntry entry in Database.GetDatabase())
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Toggle(true);
                EditorGUILayout.LabelField(entry.ID.ToString());
                foreach (Locale locale in UnityEditor.Localization.LocalizationEditorSettings.GetLocales())
                {
                    var localTable = collection.GetTable(locale.Identifier) as StringTable;
                    string localName = localTable.GetEntry(entry.ID.ToString()).LocalizedValue;
                    string newName = EditorGUILayout.TextField(locale.ToString() + " : ", localName);
                    if (localName != newName)
                    {
                        localTable.AddEntry(entry.ID.ToString(), newName);
                        EditorUtility.SetDirty(localTable);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add New"))
            {
            }
            if (GUILayout.Button("Delete Selected"))
            {
            }
            if (GUILayout.Button("Copy Selected"))
            {
            }
            EditorGUILayout.EndHorizontal();
        }

        WordDatabase Database
        {
            get
            {
                if (database == null)
                {
                    database = (WordDatabase)AssetDatabase.LoadAssetAtPath(DATABASE_PATH, typeof(WordDatabase));
                }
                return database;
            }
        }
    }
}