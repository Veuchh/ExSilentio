using LW.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;

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

        Color orangeColor => new Color(2, 1f, 0);

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

        #region LeftSide
        private void DisplayLeftSide()
        {
            GUILayout.BeginArea(new Rect(0, 0, position.width / 2, position.height - 25));
            DisplayTopButtons();
            DisplayEntries();
            GUILayout.EndArea();
        }

        private void DisplayEntries()
        {
            leftScrollPositon = GUILayout.BeginScrollView(leftScrollPositon, false, false, GUIStyle.none, GUI.skin.verticalScrollbar);

            var collection = LocalizationEditorSettings.GetStringTableCollection(STRING_TABLE_NAME);

            if (window == null)
                window = GetWindow(typeof(WordDatabaseTool));

            GUILayoutOption[] buttonOptions = new GUILayoutOption[]
            {
            GUILayout.MinWidth(window.position.width/7),
            GUILayout.MaxWidth(window.position.width/7)
            };

            foreach (WordDatabaseEntry entry in Database.GetDatabase())
            {
                DisplayEntry(collection, entry, buttonOptions);
            }

            EditorGUILayout.EndScrollView();
        }

        private void DisplayEntry(StringTableCollection collection, WordDatabaseEntry entry, GUILayoutOption[] buttonOptions)
        {
            EditorGUILayout.BeginHorizontal();

            int index = Database.GetDatabase().IndexOf(entry);


            //Displays a toggle to select the entry
            float labelWidthBuffer = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 1;
            Entries[index].IsSelected = EditorGUILayout.Toggle(Entries[index].IsSelected, toggleOptions);
            EditorGUIUtility.labelWidth = labelWidthBuffer;


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

                string newName = NoSpaceTextField(locale.ToString() + " : ", localName);

                if (localName != newName)
                {
                    localTable.AddEntry(entry.ID.ToString(), newName);
                    EditorUtility.SetDirty(localTable);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private static void DisplayTopButtons()
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
                int selectedItems = 0;

                foreach (ToolEntry entry in entries)
                {
                    if (entry.IsSelected)
                    {
                        selectedItems++;
                    }
                }

                if (selectedItems <= 0)
                {
                    EditorGUILayout.EndHorizontal();
                    return;
                }

                //Disable interaction on this window
                isInteractionDisabled = true;

                //Pop open a confirmation window
                DeleteWindow.ShowWindow(DeleteSelectedElements, OnPopupClosed, selectedItems);
            }

            if (GUILayout.Button("Copy Selected"))
            {

                List<WordDatabaseEntry> entriesToCopy = new List<WordDatabaseEntry>();

                for (int i = 0; i < entries.Count; i++)
                {
                    if (entries[i].IsSelected)
                        entriesToCopy.Add(Database.GetDatabase()[i]);
                }

                //Check if have 1 and only 1 selected item
                if (entriesToCopy.Count <= 0)
                {
                    EditorGUILayout.EndHorizontal();
                    return;
                }
                isInteractionDisabled = true;

                //Pop open a window asking to name the keys
                CopyWindow.ShowWindow(entriesToCopy, OnCopyConfirmed, OnPopupClosed);
            }
            EditorGUILayout.EndHorizontal();
        }
        #endregion LeftSide

        #region RightSide
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

                rightScrollPositon = GUILayout.BeginScrollView(rightScrollPositon, false, false, GUIStyle.none, GUI.skin.verticalScrollbar);

                for (int i = 0; i < Database.GetDatabase().Count; i++)
                {
                    if (Database.GetDatabase()[i].ID == entry.ID)
                        continue;

                    GUILayout.BeginHorizontal();

                    //Display button for additional IDs
                    bool isIDAccepted = entry.AdditionalAcceptedIds.Contains(Database.GetDatabase()[i].ID);
                    GUI.backgroundColor = isIDAccepted ? Color.green : Color.red;
                    string buttonText = isIDAccepted ? "✔" : "X";
                    if (GUILayout.Button(buttonText, GUILayout.Width(50)))
                    {
                        if (isIDAccepted)
                        {
                            entry.AdditionalAcceptedIds.Remove(Database.GetDatabase()[i].ID);
                        }
                        else
                        {
                            if (entry.CloselySemanticIds.Contains(Database.GetDatabase()[i].ID))
                            {
                                entry.CloselySemanticIds.Remove(Database.GetDatabase()[i].ID);
                            }
                            entry.AdditionalAcceptedIds.Add(Database.GetDatabase()[i].ID);
                        }
                        isModified = true;
                    }

                    //Display button for semantically close words
                    bool isIDSemanticallyClose = entry.CloselySemanticIds.Contains(Database.GetDatabase()[i].ID);
                    GUI.backgroundColor = isIDSemanticallyClose ? orangeColor : Color.red;
                    buttonText = isIDSemanticallyClose ? "✔" : "X";
                    if (GUILayout.Button(buttonText, GUILayout.Width(50)))
                    {
                        if (isIDSemanticallyClose)
                        {
                            entry.CloselySemanticIds.Remove(Database.GetDatabase()[i].ID);
                        }
                        else
                        {
                            if (entry.AdditionalAcceptedIds.Contains(Database.GetDatabase()[i].ID))
                            {
                                entry.AdditionalAcceptedIds.Remove(Database.GetDatabase()[i].ID);
                            }
                            entry.CloselySemanticIds.Add(Database.GetDatabase()[i].ID);
                        }
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
        #endregion RightSide

        #region Logic
        static bool HasSpecialChars(string yourString)
        {
            return yourString.Any(ch => !char.IsLetterOrDigit(ch));
        }

        static void ReloadEntries()
        {
            entries = new List<ToolEntry>();

            foreach (WordDatabaseEntry entry in Database.GetDatabase())
            {
                entries.Add(new ToolEntry());
            }
        }
        private static void HideCurrentlyDisplayedEntry()
        {
            foreach (ToolEntry entry in Entries)
            {
                if (entry.IsDisplayed)
                {
                    entry.IsDisplayed = false;
                }
            }
        }
        private void SelectDisplayedEntry(int index)
        {
            selectedEntry = Entries[index];
            Entries[index].IsDisplayed = true;
        }
        #endregion Logic

        #region Callbacks
        static void OnPopupClosed()
        {
            isInteractionDisabled = false;
        }

        static void DeleteSelectedElements()
        {
            List<string> enumsToRemove = new List<string>();

            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].IsSelected)
                {
                    enumsToRemove.Add(Database.GetDatabase()[i].ID.ToString());
                }
            }

            var collection = LocalizationEditorSettings.GetStringTableCollection(STRING_TABLE_NAME);


            // string[] lines = File.ReadAllLines("Assets/Core/Data/Scripts/WordID.cs");
            for (int entryIndex = 0; entryIndex < enumsToRemove.Count; entryIndex++)
            {
                //Remove it from the database
                WordID parsedEnum = (WordID)Enum.Parse(typeof(WordID), enumsToRemove[entryIndex]);
                Database.RemoveEntryByID(parsedEnum);

                //Remove it from the translation
                collection.RemoveEntry(enumsToRemove[entryIndex]);

            }
            //File.WriteAllLines("Assets/Core/Data/Scripts/WordID.cs", lines);

            //Save scriptableObjects
            EditorUtility.SetDirty(Database);
            EditorUtility.SetDirty(collection);

            HideCurrentlyDisplayedEntry();
        }

        static void OnShowAddConfirmed(List<string> newKeysNames)
        {
            OnPopupClosed();

            int nextEnumValue = Enum.GetNames(typeof(WordID)).Length;

            for (int i = 0; i < newKeysNames.Count; i++)
            {
                newKeysNames[i] = newKeysNames[i].Replace(" ", "");
                if (string.IsNullOrEmpty(newKeysNames[i]) || Enum.GetNames(typeof(WordID)).Contains(newKeysNames[i]) || HasSpecialChars(newKeysNames[i].Replace("_", "")))
                    continue;

                //Edit the enum delaration script
                string[] lines = File.ReadAllLines("Assets/Core/Data/Scripts/WordID.cs");
                lines[lines.Count() - 3] += $", {newKeysNames[i]}";
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
                //Save the scriptableobjects
                EditorUtility.SetDirty(Database);
                EditorUtility.SetDirty(collection);
            }

            //Recompile for the new enum
            CompilationPipeline.RequestScriptCompilation();
        }

        static void OnCopyConfirmed(List<string> newKeysNames, List<WordDatabaseEntry> entriesToCopy)
        {
            if (entriesToCopy.Count != newKeysNames.Count)
            {
                Debug.LogError("The amount of new names and capied entries did not match. No entry was copied");
                return;
            }

            int nextEnumValue = Enum.GetNames(typeof(WordID)).Length;

            for (int i = 0; i < newKeysNames.Count; i++)
            {
                WordDatabaseEntry newEntry = new WordDatabaseEntry();

                newKeysNames[i] = newKeysNames[i].Replace(" ", "");
                if (string.IsNullOrEmpty(newKeysNames[i]) || Enum.GetNames(typeof(WordID)).Contains(newKeysNames[i]) || HasSpecialChars(newKeysNames[i].Replace("_", "")))
                    continue;

                //Edit the enum delaration script
                string[] lines = File.ReadAllLines("Assets/Core/Data/Scripts/WordID.cs");
                lines[lines.Count() - 3] += $", {newKeysNames[i]}";
                File.WriteAllLines("Assets/Core/Data/Scripts/WordID.cs", lines);

                var collection = LocalizationEditorSettings.GetStringTableCollection(STRING_TABLE_NAME);

                //Create a new translation entry for the newly added enum
                foreach (Locale locale in LocalizationEditorSettings.GetLocales())
                {
                    var localTable = collection.GetTable(locale.Identifier) as StringTable;
                    localTable.AddEntry(newKeysNames[i], "");
                }

                newKeysNames[i] = newKeysNames[i].Replace(" ", "");
                if (string.IsNullOrEmpty(newKeysNames[i]) || Enum.GetNames(typeof(WordID)).Contains(newKeysNames[i]))
                {
                    Debug.LogWarning($"Entry {entriesToCopy[i]} did not have a valid naming conterpart, and wasn't copied");
                    continue;
                }

                newEntry.ID = (WordID)nextEnumValue;
                nextEnumValue++;
                newEntry.GdNotes = entriesToCopy[i].GdNotes;
                newEntry.AdditionalAcceptedIds = entriesToCopy[i].AdditionalAcceptedIds;
                newEntry.CloselySemanticIds = entriesToCopy[i].CloselySemanticIds;

                Database.AddEntry(newEntry);
                //Save the scriptableobjects
                EditorUtility.SetDirty(Database);
                EditorUtility.SetDirty(collection);
            }

            CompilationPipeline.RequestScriptCompilation();
        }
        #endregion Callbacks

        /// <summary>
        /// Create a EditorGUILayout.TextField with no space between label and text field
        /// </summary>
        public static string NoSpaceTextField(string label, string text)
        {
            var textDimensions = GUI.skin.label.CalcSize(new GUIContent(label));
            EditorGUIUtility.labelWidth = textDimensions.x;
            return EditorGUILayout.TextField(label, text);
        }

        GUILayoutOption[] toggleOptions = new GUILayoutOption[]
        {
            GUILayout.MinWidth(0),
            GUILayout.ExpandWidth(false)
        };

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