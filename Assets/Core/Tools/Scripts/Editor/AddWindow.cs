using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;

namespace LW.Editor.Tools.WordDatabaseTool
{
    public class AddWindow : EditorWindow
    {
        static Action<List<string>> validateCallback;
        static Action popupClosedCallback;
        static List<string> list = new List<string>();
        static EditorWindow window;
        Vector2 scrollPos;

        public static EditorWindow ShowWindow(Action<List<string>> newValidateCallback, Action newPopupClosedCallback)
        {
            validateCallback = newValidateCallback;
            popupClosedCallback = newPopupClosedCallback;
            window = GetWindow(typeof(AddWindow));
            return window;
        }

        private void OnGUI()
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos);

            for (int i = 0; i < list.Count; i++)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("-", GUILayout.Width(50)))
                {
                    list.RemoveAt(i);
                    break;
                }
                list[i] = EditorGUILayout.TextField(list[i]);
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();

            if (GUILayout.Button("+"))
            {
                list.Add("");
            }

            GUILayout.FlexibleSpace();

            //DisplayBottomButtuns
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Confirm"))
            {
                validateCallback?.Invoke(list);
                window.Close();
            }
            else if (GUILayout.Button("Cancel"))
            {
                popupClosedCallback?.Invoke();
                window.Close();
            }
            GUILayout.EndHorizontal();
        }

        private void OnDestroy()
        {
            popupClosedCallback?.Invoke();
        }
    }
}