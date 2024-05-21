using LW.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LW.Editor.Tools
{
    public class HintsTool : EditorWindow
    {
        const string DATABASE_PATH = "Assets/Core/Data/ScriptableObjects/HintDatabase.asset";
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

        [MenuItem("Windows/Hints Tool")]
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

            string keyNewValue = EditorGUILayout.TextField("Key", keyBuffer);

            if (keyBuffer != keyNewValue && !IsValueAlreadyInDB(keyNewValue))
            {
                Database.HintKeys[hintIndex] = keyNewValue;
            }
            else if (keyBuffer != keyNewValue && IsValueAlreadyInDB(keyNewValue))
            {
                Debug.LogWarning("An item of this name already exists");
            }

            GUILayout.EndHorizontal();
        }

        private bool IsValueAlreadyInDB(string keyNewValue)
        {
            return Database.HintKeys.Where(s => s == keyNewValue).Count() > 0;
        }
    }
}