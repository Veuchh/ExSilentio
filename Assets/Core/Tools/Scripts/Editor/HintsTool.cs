using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LW.Editor.Tools
{
    public class HintsTool : EditorWindow
    {
        static HintsTool window;
        static List<string> hintKeys;
        static List<string> HintKeys
        {
            get
            {
                return hintKeys;
            }
        }

        [MenuItem("Windows/Hints Tool")]
        public static void ShowWindow()
        {
            window = (HintsTool)GetWindow(typeof(HintsTool));
        }

        private void OnGUI()
        {

        }
    }
}