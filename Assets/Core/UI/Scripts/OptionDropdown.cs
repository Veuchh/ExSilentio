using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LW.UI
{
    public class OptionDropdown : OptionBase
    {
        [SerializeField] Button leftButton;
        [SerializeField] Button rightButton;
        [SerializeField] TextMeshProUGUI display;
        [SerializeField] List<string> options;

        int currentIndex = 0;

        public string CurrentlySelectedString => options[currentIndex];

        private void Awake()
        {
            leftButton.onClick.AddListener(() => OnButtonCliked(-1));
            rightButton.onClick.AddListener(() => OnButtonCliked(1));
            UpdateText();
        }

        private void OnDestroy()
        {
            leftButton.onClick.RemoveAllListeners();
            rightButton.onClick.RemoveAllListeners();
        }

        public void Init(string currentlySelectedString)
        {
            currentIndex = 0;

            if (options.Contains(currentlySelectedString))
                currentIndex = options.IndexOf(currentlySelectedString);

            UpdateText();
        }

        public void OnButtonCliked(int delta)
        {
            if (options.Count == 0)
                return;

            currentIndex += delta;
            
            if (currentIndex < 0)
                currentIndex = options.Count - 1;
            else if (currentIndex >= options.Count)
                currentIndex = 0;

            UpdateText();

            OnValueChanged();
        }

        void UpdateText()
        {
            display.text = options[currentIndex];
        }
    }
}