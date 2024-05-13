using LW.Editor.Tools.WordDatabaseTool;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LW.Editor.Tools
{

    public class TextureTestWindow : EditorWindow
    {
        static TextureTestWindow textureTestWindow;
        static string input;
        static bool createVertical = false;

        [MenuItem("Window/Texture test")]
        public static EditorWindow ShowWindow()
        {
            textureTestWindow = GetWindow<TextureTestWindow>();
            return textureTestWindow;
        }

        private void OnGUI()
        {
            input = EditorGUILayout.TextField(input);
            createVertical = EditorGUILayout.Toggle("Vertical Texture", createVertical);

            if (GUILayout.Button("Generate Texture"))
            {
                Texture2D texture = StringToTextureTest.GetTextureFromInput(input);

                //then Save To Disk as PNG
                byte[] bytes = texture.EncodeToPNG();
                var dirPath = Application.dataPath + "/Core/Tools/Textures/";
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                File.WriteAllBytes(dirPath + input + ".png", bytes);

                AssetDatabase.Refresh();
            }
        }
    }

}