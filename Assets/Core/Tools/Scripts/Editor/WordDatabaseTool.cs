using LW.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEditor;
using UnityEditor.Localization;
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

        static WordDatabase database;
        static Vector2 scrollPositon;

        static List<ToolEntry> entries = new List<ToolEntry>();
        ToolEntry selectedEntry = null;

        [MenuItem("Window/Tool GD")]
        public static void ShowWindow()
        {
            window = GetWindow(typeof(WordDatabaseTool));
            ReloadEntries();
        }

        void OnGUI()
        {
            DisplayLeftSide();
            DisplayRightSide();
        }

        private void DisplayRightSide()
        {
            GUILayout.BeginArea(new Rect(position.width / 2, position.height / 2, position.width / 2, position.height / 2));

            if (selectedEntry == null)
            {
                //Display tutorial on the tool
            }
            else
            {
                bool isModified = false;
                GUILayout.BeginVertical();
                //Display the entry

                //Display search bar

                //Display title
                WordDatabaseEntry entry = Database.GetDatabase()[entries.IndexOf(selectedEntry)];
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField(entry.ID.ToString(), EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();

                var collection = UnityEditor.Localization.LocalizationEditorSettings.GetStringTableCollection(STRING_TABLE_NAME);

                for (int i = 0; i < entry.AdditionalAcceptedIds.Count; i++)
                {
                    GUILayout.BeginHorizontal();

                    //Button to remove the accepted id
                    if (GUILayout.Button("-"))
                    {

                    }

                    //Displays the key
                    entry.AdditionalAcceptedIds[i] = (WordID)EditorGUILayout.EnumPopup(entry.AdditionalAcceptedIds[i]);

                    //Displays the translated keys
                    foreach (Locale locale in LocalizationEditorSettings.GetLocales())
                    {
                        var localTable = collection.GetTable(locale.Identifier) as StringTable;
                        EditorGUILayout.LabelField(localTable.GetEntry(entry.ID.ToString()).LocalizedValue);
                    }
                    GUILayout.EndHorizontal();
                }

                //Add another additional id
                if (GUILayout.Button("+"))
                {

                }

                //Display designer notes

                GUILayout.EndVertical();

                //Saves entry and sets the database as dirty
                if (isModified)
                {
                    Database.UpdateEntry(entry);
                    EditorUtility.SetDirty(Database);
                }
            }

            GUILayout.EndArea();
        }

        private void DisplayLeftSide()
        {
            GUILayout.BeginArea(new Rect(0, 0, position.width / 2, position.height / 2));
            DisplayBottomButtons();
            DisplayEntries();
            GUILayout.EndArea();
        }

        static void ReloadEntries()
        {
            entries = new List<ToolEntry>();

            foreach (WordDatabaseEntry entry in Database.GetDatabase())
            {
                entries.Add(new ToolEntry());
            }
        }

        private void DisplayEntries()
        {
            scrollPositon = EditorGUILayout.BeginScrollView(scrollPositon);

            var collection = UnityEditor.Localization.LocalizationEditorSettings.GetStringTableCollection(STRING_TABLE_NAME);

            string debug = "";

            foreach (WordDatabaseEntry entry in Database.GetDatabase())
            {
                DisplayEntry(collection, entry);
            }
            Debug.Log(debug);
            EditorGUILayout.EndScrollView();
        }

        private void HideCurrentlyDisplayedEntry()
        {
            foreach (ToolEntry entry in entries)
            {
                if (entry.IsDisplayed)
                {
                    entry.IsDisplayed = false;
                }
            }
        }

        private void DisplayEntry(StringTableCollection collection, WordDatabaseEntry entry)
        {
            EditorGUILayout.BeginHorizontal();

            int index = Database.GetDatabase().IndexOf(entry);

            GUILayoutOption[] toggleOptions = new GUILayoutOption[]
    {
            GUILayout.MinWidth(0),
            GUILayout.ExpandWidth(false)
        //add more layout options
    };

            //Displays a toggle to select the entry
            float labelWidthBuffer = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 1;
            entries[index].IsSelected = EditorGUILayout.Toggle(entries[index].IsSelected, toggleOptions);
            EditorGUIUtility.labelWidth = labelWidthBuffer;

            GUILayoutOption[] buttonOptions = new GUILayoutOption[]
        {
            GUILayout.MinWidth(position.width/7),
            GUILayout.MaxWidth(position.width/7)
            //add more layout options
        };

            //colors the button to green if it is the currently selected entry
            if (entries[index].IsDisplayed)
            {
                GUI.backgroundColor = Color.green;
            }
            //Button to select entry
            if (GUILayout.Button(entry.ID.ToString(), buttonOptions))
            {
                HideCurrentlyDisplayedEntry();

                SelectDisplayedEntry(index);
                //Select the entry
            }

            GUI.backgroundColor = Color.white;

            foreach (Locale locale in LocalizationEditorSettings.GetLocales())
            {
                var localTable = collection.GetTable(locale.Identifier) as StringTable;
                string localName = localTable.GetEntry(entry.ID.ToString()).LocalizedValue;

                string newName = TextField(locale.ToString() + " : ", localName);

                if (localName != newName)
                {
                    localTable.AddEntry(entry.ID.ToString(), newName);
                    EditorUtility.SetDirty(localTable);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void SelectDisplayedEntry(int index)
        {
            selectedEntry = entries[index];
            entries[index].IsDisplayed = true;
        }

        /// <summary>
        /// Create a EditorGUILayout.TextField with no space between label and text field
        /// </summary>
        public static string TextField(string label, string text)
        {
            var textDimensions = GUI.skin.label.CalcSize(new GUIContent(label));
            EditorGUIUtility.labelWidth = textDimensions.x;
            return EditorGUILayout.TextField(label, text);
        }

        private static void DisplayBottomButtons()
        {
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

        static WordDatabase Database
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