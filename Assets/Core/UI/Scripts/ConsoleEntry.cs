using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LW.UI
{
    public class ConsoleEntry : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI textField;
        [SerializeField] Image background;
        [SerializeField] Button button;

        public void UpdateText(string newHistory, bool isClickablePath = false)
        {
            textField.text = newHistory;

            if (isClickablePath)
            {
                textField.text = "<u>" + textField.text + @"</u>";
                button.interactable = true;
                string itemPath = newHistory;   // explorer doesn't like front slashes

                var l = itemPath.Split('/');

                itemPath = "";

                for (int i = 0; i < l.Length; i++)
                {
                    itemPath += l[i] + '\\';
                }

                button.onClick.AddListener(() => OpenInWinFileBrowser(itemPath));
            }
        }

        public void OpenInWinFileBrowser(string path)
        {
            bool openInsidesOfFolder = false;

            // try windows
            string winPath = path.Replace("/", @"\"); // windows explorer doesn't like forward slashes

            if (Directory.Exists(winPath)) // if path requested is a folder, automatically open insides of that folder
            {
                openInsidesOfFolder = true;
            }
            try
            {
                System.Diagnostics.Process.Start("explorer.exe", (openInsidesOfFolder ? "/root," : "/select,") + winPath);
            }
            catch (System.ComponentModel.Win32Exception e)
            {
                // tried to open win explorer in mac
                // just silently skip error
                // we currently have no platform define for the current OS we are in, so we resort to this
                e.HelpLink = ""; // do anything with this variable to silence warning about not using it
            }
        }

        private void OnDestroy()
        {
            button.onClick.RemoveAllListeners();
        }

        public void UpdateTextColor(Color color)
        {
            textField.color = color;
        }
        public void UpdateBackgroundColor(Color color)
        {
            background.color = color;
        }

    }
}