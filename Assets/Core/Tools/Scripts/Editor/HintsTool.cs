using LW.Data;
using System.Linq;
using UnityEditor;
using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;

namespace LW.Editor.Tools
{
    public class HintsTool : EditorWindow
    {
        const string DATABASE_PATH = "Assets/Core/Data/ScriptableObjects/HintDatabase.asset";
        const string STRING_TABLE_NAME = "TextTable";
        static HintsTool window;
        static HintDatabase database;

        Vector2 scrollPos;

        static HintDatabase Database
        {
            get
            {
                if (database == null)
                    database = (HintDatabase)AssetDatabase.LoadAssetAtPath(DATABASE_PATH, typeof(HintDatabase));
                return database;
            }
        }

        [MenuItem("Window/Hints Tool")]
        public static void ShowWindow()
        {
            window = (HintsTool)GetWindow(typeof(HintsTool));
        }

        private void OnGUI()
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos);

            for (int hintIndex = 0; hintIndex < Database.HintKeys.Count; hintIndex++)
                DisplayEntry(hintIndex);

            GUILayout.EndScrollView();
        }

        private void DisplayEntry(int hintIndex)
        {
            GUILayout.BeginHorizontal();

            string keyBuffer = Database.HintKeys[hintIndex];
            string keyNewValue = NoSpaceTextField("Key", keyBuffer);
            var collection = LocalizationEditorSettings.GetStringTableCollection(STRING_TABLE_NAME);
            string hintTranslationKey = ((StringTable)collection.GetTable(LocalizationEditorSettings.GetLocales()[0].Identifier)).GetEntry(Database.HintKeys[hintIndex].ToString())?.Key;

            hintTranslationKey = (string.IsNullOrEmpty(hintTranslationKey) ? "" : hintTranslationKey);

            if (keyBuffer != keyNewValue && !IsValueAlreadyInDB(keyNewValue) && hintTranslationKey != keyNewValue)
            {
                Database.HintKeys[hintIndex] = keyNewValue;

                foreach (Locale locale in LocalizationEditorSettings.GetLocales())
                {
                    var localTable = collection.GetTable(locale.Identifier) as StringTable;
                    //Create entry
                    if (localTable.GetEntry(keyBuffer) == null)
                        localTable.AddEntry(keyNewValue, "");
                //Update entry
                    else
                    {
                        localTable.AddEntry(keyNewValue, localTable.GetEntry(keyBuffer).LocalizedValue);
                        localTable.RemoveEntry(keyBuffer);
                    }
                    EditorUtility.SetDirty(localTable);
                }
            }
            else if (keyBuffer != keyNewValue && (IsValueAlreadyInDB(keyNewValue) || hintTranslationKey == keyNewValue))
            {
                Debug.LogWarning("An item of this name already exists");
            }

            foreach (Locale locale in LocalizationEditorSettings.GetLocales())
            {
                var localTable = collection.GetTable(locale.Identifier) as StringTable;

                string newHint = NoSpaceTextField(locale.ToString() + " : ", localTable.GetEntry(Database.HintKeys[hintIndex].ToString()).LocalizedValue);

                if (hintTranslationKey != newHint)
                {
                    localTable.AddEntry(Database.HintKeys[hintIndex].ToString(), newHint);
                    EditorUtility.SetDirty(localTable);
                }
            }

            GUILayout.EndHorizontal();
        }

        private bool IsValueAlreadyInDB(string keyNewValue)
        {
            return Database.HintKeys.Where(s => s == keyNewValue).Count() > 0;
        }

        /// <summary>
        /// Create a EditorGUILayout.TextField with no space between label and text field
        /// </summary>
        public static string NoSpaceTextField(string label, string text, GUILayoutOption option = null)
        {
            var textDimensions = GUI.skin.label.CalcSize(new GUIContent(label));
            EditorGUIUtility.labelWidth = textDimensions.x;
            return EditorGUILayout.TextField(label, text);
        }
    }
}