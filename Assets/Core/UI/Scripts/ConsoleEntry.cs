using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LW.UI
{
    public class ConsoleEntry : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI textField;
        [SerializeField] Image background;
        public void UpdateText(string newHistory)
        {
            textField.text = newHistory;
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