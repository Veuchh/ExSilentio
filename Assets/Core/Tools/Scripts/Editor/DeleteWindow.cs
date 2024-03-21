using System;
using UnityEditor;
using UnityEngine;

namespace LW.Editor.Tools.WordDatabaseTool
{
    public class DeleteWindow : EditorWindow
    {
        static EditorWindow deleteWindow;
        static Action deleteSelectedElementsCallback;
        static Action onPopupClosedCallback;
        static int elementAmount;

        public static EditorWindow ShowWindow(Action deleteSelectedElements, Action onPopupClosed, int newElementAmount)
        {
            deleteWindow = GetWindow<DeleteWindow>();
            elementAmount = newElementAmount;
            deleteSelectedElementsCallback = deleteSelectedElements;
            onPopupClosedCallback = onPopupClosed;
            return deleteWindow;
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField($"Do you really want to delete {elementAmount} entries ?");

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Yes"))
            {
                deleteSelectedElementsCallback?.Invoke();
                deleteWindow.Close();
            }

            if (GUILayout.Button("No"))
            {
                onPopupClosedCallback?.Invoke();
                deleteWindow.Close();
            }

            GUILayout.EndHorizontal();
        }

        private void OnDestroy()
        {
            onPopupClosedCallback?.Invoke();
        }
    }
}