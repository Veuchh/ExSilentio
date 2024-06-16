using System.Collections.Generic;
using System.Collections.ObjectModel;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Tables;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

namespace LW.UI
{
    public class OptionDropdown : OptionBase
    {
        [SerializeField] Button leftButton;
        [SerializeField] Button rightButton;
        [SerializeField] TextMeshProUGUI display;
        [SerializeField] LocalizeStringEvent stringEvent;
        [SerializeField] LocalizedStringTable stringTable;
        [SerializeField] List<string> options;

        int currentIndex = 0;

        public string CurrentlySelectedString => options[currentIndex];

        protected override void Awake()
        {
            base.Awake();

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
            stringEvent.StringReference = new LocalizedString(stringTable.GetTable().SharedData.TableCollectionNameGuid, stringTable.GetTable().GetEntry(options[currentIndex]).KeyId);
        }
    }
}