using LW.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LW.Editor.Tools.WordDatabaseTool
{
    public class CopyWindow : EditorWindow
    {
        static CopyWindow window;
        static Action<List<string>, List<WordDatabaseEntry>> copyConfirmCallback;
        static Action windowClosedCallback;
        Vector2 scrollValue;
        static List<WordDatabaseEntry> entriesToCopy = new List<WordDatabaseEntry>();
        static List<string> newNames = new List<string>();
        public static CopyWindow ShowWindow(List<WordDatabaseEntry> newEntriesToCopy, Action<List<string>, List<WordDatabaseEntry>> onCopyConfirmCallback, Action onWindowClosedCallback)
        {
            window = GetWindow<CopyWindow>();
            entriesToCopy = newEntriesToCopy;
            copyConfirmCallback = onCopyConfirmCallback;
            windowClosedCallback = onWindowClosedCallback;

            foreach (var item in entriesToCopy)
            {
                newNames.Add("");
            }

            return window;
        }

        private void OnGUI()
        {
            scrollValue = GUILayout.BeginScrollView(scrollValue, false, false, GUIStyle.none, GUI.skin.verticalScrollbar);
            for (int i = 0; i < entriesToCopy.Count; i++)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(entriesToCopy[i].ID.ToString(), GUILayout.Width(window.position.size.x /5));
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField("=>", GUILayout.Width(50));
                GUILayout.FlexibleSpace();
                newNames[i] = NoSpaceTextField("", newNames[i], GUILayout.Width(4*(window.position.size.x / 3) - 50));
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Confirm"))
            {
                copyConfirmCallback?.Invoke(newNames, entriesToCopy);
                window?.Close();
            }
            if (GUILayout.Button("Cancel"))
            {
                window?.Close();
            }
            GUILayout.EndHorizontal();
        }

        private void OnDestroy()
        {
            windowClosedCallback?.Invoke();
        }

        /// <summary>
        /// Create a EditorGUILayout.TextField with no space between label and text field
        /// </summary>
        public static string NoSpaceTextField(string label, string text, GUILayoutOption option)
        {
            var textDimensions = GUI.skin.label.CalcSize(new GUIContent(label));
            EditorGUIUtility.labelWidth = textDimensions.x;
            return EditorGUILayout.TextField(label, text, option);
        }
    }
}