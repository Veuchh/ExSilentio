using Codice.CM.Common;
using LW.Data;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.Localization;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using static UnityEngine.EventSystems.EventTrigger;

namespace LW.Editor.Tools.WordDatabaseTool
{
    public class WordDatabaseTool : EditorWindow
    {
        const string DATABASE_PATH = "Assets/Core/Data/ScriptableObjects/WordDatabase.asset";
        const string STRING_TABLE_NAME = "TextTable";

        static EditorWindow window;

        static WordDatabase database;
        static Vector2 leftScrollPositon;
        static Vector2 rightScrollPositon;

        static List<ToolEntry> entries;
        ToolEntry selectedEntry = null;
        static bool isInteractionDisabled = false;

        [MenuItem("Window/Tool GD")]
        public static void ShowWindow()
        {
            window = GetWindow(typeof(WordDatabaseTool));
            ReloadEntries();
            isInteractionDisabled = false;
        }

        void OnGUI()
        {
            if (isInteractionDisabled)
                GUI.enabled = false;
            DisplayLeftSide();
            DisplayRightSide();
        }

        private void DisplayRightSide()
        {
            GUILayout.BeginArea(new Rect(position.width / 2, 0, position.width / 2, position.height));

            if (selectedEntry == null)
            {
                //Display tutorial on the tool
            }
            else
            {
                bool isModified = false;
                WordDatabaseEntry entry = Database.GetDatabase()[Entries.IndexOf(selectedEntry)];
                var collection = LocalizationEditorSettings.GetStringTableCollection(STRING_TABLE_NAME);

                //Display the entry

                //Display search bar

                //Display title
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(entry.ID.ToString(), EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.Space(10);

                //Display designer notes
                entry.GdNotes = EditorGUILayout.TextArea(entry.GdNotes);

                GUILayout.Space(10);

                //Display the additional ids
                GUILayout.Label("Additional Inputs");

                GUILayout.Space(5);

                string bufferNotes = entry.GdNotes;

                if (bufferNotes != entry.GdNotes)
                    isModified = true;

                GUILayout.Space(5);

                rightScrollPositon = GUILayout.BeginScrollView(rightScrollPositon);

                for (int i = 0; i < Database.GetDatabase().Count; i++)
                {
                    if (Database.GetDatabase()[i].ID == entry.ID)
                        continue;

                    GUILayout.BeginHorizontal();
                    bool isIDAccepted = entry.AdditionalAcceptedIds.Contains(Database.GetDatabase()[i].ID);

                    GUI.backgroundColor = isIDAccepted ? Color.green : Color.red;
                    string buttonText = isIDAccepted ? "✔" : "X";
                    if (GUILayout.Button(buttonText))
                    {
                        if (isIDAccepted)
                            entry.AdditionalAcceptedIds.Remove(Database.GetDatabase()[i].ID);
                        else
                            entry.AdditionalAcceptedIds.Add(Database.GetDatabase()[i].ID);
                        isModified = true;
                    }
                    GUI.backgroundColor = Color.white;

                    //Displays the key
                    EditorGUILayout.LabelField(Database.GetDatabase()[i].ID.ToString());

                    //Displays the translated keys
                    foreach (Locale locale in LocalizationEditorSettings.GetLocales())
                    {
                        var localTable = collection.GetTable(locale.Identifier) as StringTable;
                        EditorGUILayout.LabelField(localTable.GetEntry(Database.GetDatabase()[i].ID.ToString()).LocalizedValue);
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();


                //Saves entry and sets the database as dirty
                if (isModified)
                {
                    Database.UpdateEntry(entry);
                    EditorUtility.SetDirty(Database);
                }
                GUILayout.FlexibleSpace();
            }


            GUILayout.EndArea();
        }

        private void DisplayLeftSide()
        {
            GUILayout.BeginArea(new Rect(0, 0, position.width / 2, position.height - 25));
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
            leftScrollPositon = EditorGUILayout.BeginScrollView(leftScrollPositon);

            var collection = LocalizationEditorSettings.GetStringTableCollection(STRING_TABLE_NAME);


            foreach (WordDatabaseEntry entry in Database.GetDatabase())
            {
                DisplayEntry(collection, entry);
            }

            EditorGUILayout.EndScrollView();
        }

        private void HideCurrentlyDisplayedEntry()
        {
            foreach (ToolEntry entry in Entries)
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
            Entries[index].IsSelected = EditorGUILayout.Toggle(Entries[index].IsSelected, toggleOptions);
            EditorGUIUtility.labelWidth = labelWidthBuffer;

            GUILayoutOption[] buttonOptions = new GUILayoutOption[]
        {
            GUILayout.MinWidth(position.width/7),
            GUILayout.MaxWidth(position.width/7)
            //add more layout options
        };

            //colors the button to green if it is the currently selected entry
            if (Entries[index].IsDisplayed)
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
            selectedEntry = Entries[index];
            Entries[index].IsDisplayed = true;
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
                isInteractionDisabled = true;
                AddWindow.ShowWindow(OnShowAddConfirmed, OnPopupClosed).Focus();
            }

            if (GUILayout.Button("Delete Selected"))
            {
                //Check if have 1 or more selected item
                //Pop open a confirmation window
                //Remove every selected elements
            }

            if (GUILayout.Button("Copy Selected"))
            {
                //Check if have 1 and only 1 selected item
                //Pop open a window asking to name the key
                //Add in a new entry
                //populate the entr additional ids and note by copying those of the selected one.
            }
            EditorGUILayout.EndHorizontal();
        }

        static void OnPopupClosed()
        {
            isInteractionDisabled = false;
        }

        static void OnShowAddConfirmed(List<string> newKeysNames)
        {
            OnPopupClosed();

            int nextEnumValue = Enum.GetNames(typeof(WordID)).Length;

            for (int i = 0; i < newKeysNames.Count; i++)
            {
                newKeysNames[i] = newKeysNames[i].Replace(" ", "");
                if (string.IsNullOrEmpty(newKeysNames[i]) || Enum.GetNames(typeof(WordID)).Contains(newKeysNames[i]))
                    continue;

                //Edit the enum delaration script
                string[] lines = File.ReadAllLines("Assets/Core/Data/Scripts/WordID.cs");
                lines[lines.Count() - 3] += $"{newKeysNames[i]},";
                File.WriteAllLines("Assets/Core/Data/Scripts/WordID.cs", lines);

                var collection = LocalizationEditorSettings.GetStringTableCollection(STRING_TABLE_NAME);

                //Create a new translation entry for the newly added enum
                foreach (Locale locale in LocalizationEditorSettings.GetLocales())
                {
                    var localTable = collection.GetTable(locale.Identifier) as StringTable;
                    localTable.AddEntry(newKeysNames[i], "");
                }

                //Create a new entry
                WordDatabaseEntry newEntry = new WordDatabaseEntry();

                //Sets the correct ID for the (not compiled yet lol) enum
                newEntry.ID = (WordID)nextEnumValue;
                nextEnumValue++;

                Database.AddEntry(newEntry);
                //Save the scriptableobject
                EditorUtility.SetDirty(Database);

                //Recompile for the new enum
                CompilationPipeline.RequestScriptCompilation();
            }
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


        static List<ToolEntry> Entries
        {
            get
            {
                if (entries == null)
                {
                    ReloadEntries();
                }
                return entries;
            }
        }
    }
}