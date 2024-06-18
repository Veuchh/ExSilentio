using UnityEngine;
using TMPro;
using NaughtyAttributes;
using UnityEngine.UI;
using System;
using LW.Data;
using LW.Audio;
using System.Collections.Generic;

namespace LW.UI
{
    public class ConsoleUI : MonoBehaviour
    {
        [Header("Console")]
        [SerializeField] Canvas canvas;
        [SerializeField] CanvasGroup consoleCanvasGroup;
        [SerializeField] ScrollRect scrollRect;
        [SerializeField] VerticalLayoutGroup historyParent;
        [SerializeField] TextMeshProUGUI input;
        [SerializeField] ConsoleEntry consoleEntryPrefab;
        [SerializeField] GameObject commandPannel;
        [SerializeField] Button closeButton;
        [SerializeField] Button commandsCommand;
        [SerializeField] Button helpCommand;
        [SerializeField] Button loadCommand;
        [SerializeField] Button progressCommand;
        [SerializeField] Button hintCommand;
        [SerializeField] Button respawnCommand;
        [SerializeField] Button screenshotCommand;
        [SerializeField] Button togglePannelButton;
        [SerializeField] Vector2 toggleButtonPositions = new Vector2(-535, -838);



        [SerializeField] float cursorFlashingSpeed = 1;
        [SerializeField] bool separateWithDashes = false;
        [SerializeField] bool changeBackground = false;
        [SerializeField, ShowIf(nameof(changeBackground))] Color backgroundColor1;
        [SerializeField, ShowIf(nameof(changeBackground))] Color backgroundColor2;
        [SerializeField] bool changeTextColor = true;
        [SerializeField, ShowIf(nameof(changeTextColor))] Color textColor1;
        [SerializeField, ShowIf(nameof(changeTextColor))] Color textColor2;

        [Header("Notifications")]
        [SerializeField] CanvasGroup notificationCanvasGroup;
        [SerializeField] CanvasGroup notificationGlowCanvasGroup;
        [SerializeField] TextMeshProUGUI bindingDisplay;
        [SerializeField] Image notificationIcon;
        [SerializeField] float notificationGlowSpeed;

        [Header("Wwise Events")]
        [SerializeField] AK.Wwise.Event uiClClick;

        public static event Action onCloseClicked;
        public static event Action<CommandID, string> onCommandClicked;

        bool isEntryEven = true;
        string currentInput = string.Empty;
        float elapsedTime = 0;
        bool isConsoleShown = false;
        bool hasNotification = false;

        bool preventConsole = false;
        public bool PreventConsole => preventConsole;

        public static ConsoleUI Instance;

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            closeButton.onClick.AddListener(() => onCloseClicked?.Invoke());
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

            if (hasNotification)
            {
                notificationGlowCanvasGroup.alpha = Mathf.Abs(Mathf.Sin(Time.time * notificationGlowSpeed));
            }
        }

        private void OnDestroy()
        {
            closeButton.onClick.RemoveAllListeners();
            commandsCommand.onClick.RemoveAllListeners();
            helpCommand.onClick.RemoveAllListeners();
            loadCommand.onClick.RemoveAllListeners();
            hintCommand.onClick.RemoveAllListeners();
            progressCommand.onClick.RemoveAllListeners();
            respawnCommand.onClick.RemoveAllListeners();
            screenshotCommand.onClick.RemoveAllListeners();
            togglePannelButton.onClick.RemoveAllListeners();
        }

        public void TogglePreventConsole()
        {
            preventConsole = !preventConsole;
        }

        public void ToggleConsole(bool toggle, bool applyToAll = false)
        {
            if (preventConsole)
                return;

            canvas.sortingOrder = toggle ? 1 : -1;
            consoleCanvasGroup.alpha = toggle ? 1 : 0;

            if (applyToAll)
            {
                notificationCanvasGroup.alpha = toggle ? 1 : 0;
            }
            else
            {
                notificationCanvasGroup.alpha = toggle ? 0 : 1;
            }

            notificationGlowCanvasGroup.alpha = 0;
            UpdateInput("");
            isConsoleShown = toggle;
            notificationIcon.gameObject.SetActive(false);

            if (toggle)
                hasNotification = false;
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

            if (!isConsoleShown)
            {
                hasNotification = true;
                notificationIcon.gameObject.SetActive(true);
            }
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

            Vector3 buttonPosition = togglePannelButton.transform.localPosition;

            buttonPosition.x = commandPannel.activeSelf ? toggleButtonPositions.y : toggleButtonPositions.x;

            togglePannelButton.transform.localPosition = buttonPosition;
        }

        public void UpdateConsoleColor(List<Color> newColors)
        {
            if (newColors.Count != 4)
            {
                Debug.LogError($"newColors should have 4 values. It has {newColors.Count}");
                return;
            }

            backgroundColor1 = newColors[0];
            backgroundColor2 = newColors[1];
            textColor1 = newColors[2];
            textColor2 = newColors[3];
        }

        public void UpdateConsoleDashes(bool useDashes)
        {
            separateWithDashes = useDashes;
        }

        public void UpdateBindingDisplay(string binding)
        {
            bindingDisplay.text = binding;
        }
    }
}