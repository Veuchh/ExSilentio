using UnityEngine;
using TMPro;
using NaughtyAttributes;
using UnityEngine.UI;
using System;
using LW.Data;
using LW.Audio;

namespace LW.UI
{
    public class ConsoleUI : MonoBehaviour
    {
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] ScrollRect scrollRect;
        [SerializeField] VerticalLayoutGroup historyParent;
        [SerializeField] TextMeshProUGUI input;
        [SerializeField] ConsoleEntry consoleEntryPrefab;
        [SerializeField] GameObject commandPannel;
        [SerializeField] Button commandsCommand;
        [SerializeField] Button helpCommand;
        [SerializeField] Button loadCommand;
        [SerializeField] Button progressCommand;
        [SerializeField] Button hintCommand;
        [SerializeField] Button respawnCommand;
        [SerializeField] Button screenshotCommand;
        [SerializeField] Button togglePannelButton;

        [SerializeField] float cursorFlashingSpeed = 1;
        [SerializeField] bool separateWithDashes = false;
        [SerializeField] bool changeBackground = false;
        [SerializeField, ShowIf(nameof(changeBackground))] Color backgroundColor1;
        [SerializeField, ShowIf(nameof(changeBackground))] Color backgroundColor2;
        [SerializeField] bool changeTextColor = true;
        [SerializeField, ShowIf(nameof(changeTextColor))] Color textColor1;
        [SerializeField, ShowIf(nameof(changeTextColor))] Color textColor2;

        [Header("Wwise Events")]
        [SerializeField] AK.Wwise.Event uiClClick;

        public static event Action<CommandID, string> onCommandClicked;

        bool isEntryEven = true;
        string currentInput = string.Empty;

        float elapsedTime = 0;

        public static ConsoleUI Instance;
        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            commandsCommand.onClick.AddListener(() => OnCommandClicked(CommandID.commands));
            helpCommand.onClick.AddListener(() => OnCommandClicked(CommandID.help));
            progressCommand.onClick.AddListener(() => OnCommandClicked(CommandID.progress));
            hintCommand.onClick.AddListener(() => OnCommandClicked(CommandID.hint));
            loadCommand.onClick.AddListener(() => OnCommandClicked(CommandID.load));
            respawnCommand.onClick.AddListener(() => OnCommandClicked(CommandID.resetPosition));
            screenshotCommand.onClick.AddListener(() => OnCommandClicked(CommandID.screenshot));
            togglePannelButton.onClick.AddListener(() => TogglePannel());
        }

        private void Update()
        {
            elapsedTime += Time.deltaTime;
            input.text = currentInput + (Mathf.Sin(elapsedTime * cursorFlashingSpeed) < 0 ? "" : "|");
        }

        private void OnDestroy()
        {
            commandsCommand.onClick.RemoveAllListeners();
            helpCommand.onClick.RemoveAllListeners();
            loadCommand.onClick.RemoveAllListeners();
            hintCommand.onClick.RemoveAllListeners();
            progressCommand.onClick.RemoveAllListeners();
            respawnCommand.onClick.RemoveAllListeners();
            screenshotCommand.onClick.RemoveAllListeners();
            togglePannelButton.onClick.RemoveAllListeners();
        }

        public void ToggleConsole(bool toggle)
        {
            canvasGroup.alpha = toggle ? 1 : 0; 
            UpdateInput("");
        }

        public void AddToHistory(string newHistory, bool isClickablePath = false)
        {
            if (separateWithDashes)
                Instantiate(consoleEntryPrefab, historyParent.transform).UpdateText("------------------------------------");

            ConsoleEntry newEntry = Instantiate(consoleEntryPrefab, historyParent.transform);
            newEntry.UpdateText(newHistory, isClickablePath);

            if (changeTextColor)
                newEntry.UpdateTextColor(isEntryEven ? textColor2 : textColor1);

            if (changeBackground)
                newEntry.UpdateBackgroundColor(isEntryEven ? backgroundColor2 : backgroundColor1);

            isEntryEven = !isEntryEven;

            Canvas.ForceUpdateCanvases();

            historyParent.CalculateLayoutInputVertical();
            historyParent.GetComponent<ContentSizeFitter>().SetLayoutVertical(); 
            
            scrollRect.verticalNormalizedPosition = 0;
        }

        public void UpdateInput(string newInput)
        {
            currentInput = newInput;
            elapsedTime = 0;
        }

        public void OnCommandClicked(CommandID commandID)
        {
            onCommandClicked?.Invoke(commandID, "");
            WwiseInterface.Instance.PlayEvent(uiClClick);
        }

        void TogglePannel()
        {
            commandPannel.SetActive(!commandPannel.activeSelf);

            WwiseInterface.Instance.PlayEvent(uiClClick);

            togglePannelButton.transform.rotation = Quaternion.Euler(0, 0, commandPannel.activeSelf ? 0 : 180);
        }
    }
}