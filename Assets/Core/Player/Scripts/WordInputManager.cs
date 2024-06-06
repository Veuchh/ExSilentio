using LW.Audio;
using LW.Data;
using LW.Level;
using LW.Logger;
using LW.UI;
using LW.Word;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;

namespace LW.Player
{
    public class WordInputManager : MonoBehaviour
    {
        const string LEVEL_COMMAND_FEEDBACK_ID = "levelCommandFeedbackID";
        const string LEVELFAILED_COMMAND_FEEDBACK_ID = "levelFailedCommandFeedbackID";
        const string LEVELINTERRUPTED_COMMAND_FEEDBACK_ID = "levelInterruptedCommandFeedbackID";
        const string LEVELSUCCESS_COMMAND_FEEDBACK_ID = "levelSuccessCommandFeedbackID";
        const string RESET_POSITION_COMMAND_FEEDBACK_ID = "resetPositionCommandFeedbackID";
        const string COMMANDS_COMMAND_FEEDBACK_ID = "commandsCommandFeedbackID";
        const string HELP_COMMAND_FEEDBACK_ID = "helpCommandFeedbackID";
        const string HINT_COMMAND_FEEDBACK_ID = "hintCommandFeedbackID";
        const string REVEALED_ITEMS_LOCALIZATION_ID = "revealedItems";
        const string CORE_LOCALIZATION_ID = "Core";
        const string SECONDARY_LOCALIZATION_ID = "Secondary";
        const string BONUS_LOCALIZATION_ID = "Bonus";

        [SerializeField] LocalizedStringTable wordsTable;
        [SerializeField] LocalizedStringTable commandsTable;
        [SerializeField] WordDatabase wordDatabase;
        [SerializeField] WordCorrector wordCorrector;

        [Header("Wwise Events")]
        [SerializeField] AK.Wwise.Event uiClOpen;
        [SerializeField] AK.Wwise.Event uiClClose;
        [SerializeField] AK.Wwise.Event uiClWriteText;
        [SerializeField] AK.Wwise.Event uiClDeleteText;
        [SerializeField] AK.Wwise.Event uiClWordEnter;
        string currentWordInput;

        [Header("Input Settings")]
        [SerializeField] float startchainDeleteDelay = .2f;
        [SerializeField] float chainDeleteDelay = .05f;
        float nextDeleteTime = 0;
        bool isDeleting = false;

        //Console navigation
        List<string> previousInputs = new List<string>();
        int currentNavigationIndex;

        public static event Action onResetPlayerPos;

        private void Awake()
        {
            ConsoleUI.onCommandClicked += OnUseCommand;
        }

        private void OnDestroy()
        {
            ConsoleUI.onCommandClicked -= OnUseCommand;
        }

        private void Update()
        {
            while (isDeleting && Time.time > nextDeleteTime)
            {
                nextDeleteTime += chainDeleteDelay;
                RemoveCharacter();
            }
        }

        public void ToggleConsole(bool isToggled)
        {
            ConsoleUI.Instance.ToggleConsole(isToggled);
            ClearWord();
            ConsoleUI.Instance.UpdateInput(currentWordInput);
            WwiseInterface.Instance.PlayEvent((isToggled ? uiClOpen : uiClClose));

            StaticData.OpenWindowsAmount += isToggled ? 1 : -1;
        }

        public void ClearWord()
        {
            currentWordInput = string.Empty;
        }

        public void AddCharacter(char chr)
        {
            if (!UsableCharacters.Characters.Contains(chr))
                return;

            WwiseInterface.Instance.PlayEvent(uiClWriteText);
            currentWordInput += chr;
            ConsoleUI.Instance.UpdateInput(currentWordInput);
        }

        public void ToggleDelete(bool deleteToggled)
        {
            if (deleteToggled)
            {
                RemoveCharacter();
                nextDeleteTime = Time.time + startchainDeleteDelay;
            }

            isDeleting = deleteToggled;
        }

        void RemoveCharacter()
        {
            if (!string.IsNullOrEmpty(currentWordInput))
            {
                currentWordInput = currentWordInput.Substring(0, currentWordInput.Length - 1);
                ConsoleUI.Instance.UpdateInput(currentWordInput);
            }

            WwiseInterface.Instance.PlayEvent(uiClDeleteText);
        }

        private void OnSuccesfullParse(WordID wordID)
        {
            Debug.Log($"Parse succesful : {wordID}");

            if (RevealableObjectHandler.Instance == null)
            {
                Debug.LogWarning($"You attempted to reveal a word, but no {nameof(RevealableObjectHandler)} Instance was found.");
            }

            DatabaseQueryResult queryResult = wordDatabase.AttemptDatabaseRetrieve(wordID);

            RevealableObjectHandler.Instance.AttemptWordDiscovery(queryResult, OnWordRevealed);
        }

        private void OnFailedParse(string failedToParseString)
        {
            Debug.Log($"Parse failed");

            if (RevealableObjectHandler.Instance == null)
            {
                Debug.LogWarning($"You attempted to reveal a word, but no {nameof(RevealableObjectHandler)} Instance was found.");
            }

            RevealableObjectHandler.Instance.OnParseFailed(failedToParseString);
        }

        public void SubmitWord()
        {
            if (string.IsNullOrEmpty(currentWordInput))
                return;

            previousInputs.Add(currentWordInput);
            ResetConsoleNavigaion();

            CustomLogger.OnWordInput(currentWordInput, Vector3.zero, 0);

            if (!wordCorrector.AttemptParsingToCommand(currentWordInput, OnUseCommand))
                wordCorrector.AttemptParsingToID(currentWordInput, OnSuccesfullParse, OnFailedParse);


            WwiseInterface.Instance.PlayEvent(uiClWordEnter);
            ClearWord();
            ConsoleUI.Instance.UpdateInput(currentWordInput);
        }

        private void OnUseCommand(CommandID id, string arguments = "")
        {
            switch (id)
            {
                case CommandID.hint:
                    OnHintCommand(arguments);
                    break;
                case CommandID.progress:
                    OnProgressCommand();
                    break;
                case CommandID.help:
                    OnHelpCommand();
                    break;
                case CommandID.setSpeed:
                    OnSetSpeedCommand(arguments);
                    break;
                case CommandID.screenshot:
                    StartCoroutine(TakeScreenShotRoutine());
                    break;
                case CommandID.resetPosition:
                    OnResetPositionCommand();
                    break;
                case CommandID.commands:
                    OnCommandsCommand();
                    break;
                case CommandID.load:
                    OnLevelCommand(arguments);
                    break;
            }
        }

        private void OnLevelCommand(string args)
        {
            var translatedTable = commandsTable.GetTable();
            string consoleOutput = string.Empty;

            if (string.IsNullOrWhiteSpace(args))
            {
                foreach (string level in LevelLoader.Instance.Levels)
                {
                    consoleOutput += "- " + translatedTable.GetEntry(level).LocalizedValue + "\n";
                }

                consoleOutput += translatedTable.GetEntry(LEVEL_COMMAND_FEEDBACK_ID).LocalizedValue;
            }

            else
            {
                ICollection<StringTableEntry> entries = translatedTable.Values;
                string sceneName = string.Empty;
                string levelName = string.Empty;
                foreach (var item in entries)
                {
                    if (item.LocalizedValue.ToLower() == args.Replace(" ", "").ToLower())
                    {
                        sceneName = item.Key;
                        levelName = item.Value;
                        break;
                    }
                }

                switch (LevelLoader.Instance.ChangeLevel(sceneName, levelName))
                {
                    case FunctionResult.Success:
                        //loading scene
                        consoleOutput = translatedTable.GetEntry(LEVELSUCCESS_COMMAND_FEEDBACK_ID).LocalizedValue + levelName;
                        //close console
                        PlayerInputsHandler.Instance.ToggleWordMode();
                        break;
                    case FunctionResult.Failed:
                        //level doesn't exist
                        consoleOutput = args + translatedTable.GetEntry(LEVELFAILED_COMMAND_FEEDBACK_ID).LocalizedValue;
                        break;
                    case FunctionResult.Interrupted:
                        //already here
                        consoleOutput = translatedTable.GetEntry(LEVELINTERRUPTED_COMMAND_FEEDBACK_ID).LocalizedValue;
                        break;
                }
            }

            ConsoleUI.Instance.AddToHistory(consoleOutput);
        }

        private void OnCommandsCommand()
        {
            var translatedTable = commandsTable.GetTable();
            string consoleOutput = translatedTable.GetEntry(COMMANDS_COMMAND_FEEDBACK_ID).LocalizedValue;
            ConsoleUI.Instance.AddToHistory(consoleOutput);
        }

        private void OnResetPositionCommand()
        {
            var translatedTable = commandsTable.GetTable();
            string consoleOutput = translatedTable.GetEntry(RESET_POSITION_COMMAND_FEEDBACK_ID).LocalizedValue;
            ConsoleUI.Instance.AddToHistory(consoleOutput);

            onResetPlayerPos?.Invoke();
        }

        IEnumerator TakeScreenShotRoutine()
        {
            ConsoleUI.Instance.ToggleConsole(false);
            yield return new WaitForEndOfFrame();
            string path = Application.dataPath;

#if UNITY_EDITOR
            path += "/Assets";
#endif

            path += "/Screenshots";
            if (!Directory.Exists(path.Replace("/", @"\")))
                Directory.CreateDirectory(Application.dataPath.Replace("/", @"\") + @"\Screenshots");

            string screenNameBase = "";

#if UNITY_EDITOR
            screenNameBase += "Assets/";
#endif

            ScreenCapture.CaptureScreenshot(screenNameBase + "Screenshots/Ex_Silentio_" + DateTime.Now.ToString().Replace(" ", "_").Replace(":", "_").Replace("/", "_") + ".png");
            ConsoleUI.Instance.ToggleConsole(true);
            ConsoleUI.Instance.AddToHistory(path, true);
        }

        private void OnSetSpeedCommand(string arguments)
        {
            PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();

            if (playerMovement != null && int.TryParse(arguments, out int newSpeed))
                playerMovement.SetNewSpeed(newSpeed);
        }

        void OnHintCommand(string arguments)
        {
            var translatedWordsTable = wordsTable.GetTable();
            var translatedCommandsTable = commandsTable.GetTable();
            string consoleOutput = string.Empty;
            List<RevealableObjectBundle> bundles = RevealableObjectHandler.Instance.GetBundles();
            arguments = arguments.Replace(" ", "");
            arguments.Normalize();

            if (!string.IsNullOrEmpty(arguments) && int.TryParse(arguments, out int bundleToRevealIndex) && bundleToRevealIndex < bundles.Count)
                bundles[bundleToRevealIndex].RevealHint();


            foreach (RevealableObjectBundle bundle in bundles)
            {
                consoleOutput += $"{bundles.IndexOf(bundle)}. " +
                    (bundle.IsRevealed ?
                    translatedWordsTable.GetEntry(bundle.ID.ToString()).LocalizedValue
                    : " ?????????")
                    + " : ";
                for (int hintIndex = 0; hintIndex < bundle.Hints.Count; hintIndex++)
                {
                    consoleOutput += $"\n\t- ";
                    string hintKey = bundle.HintsBase[hintIndex];
                    if (bundle.Hints[hintKey])
                    {
                        string translatedHint = translatedWordsTable.GetEntry(hintKey).LocalizedValue;
                        consoleOutput += translatedHint;
                    }
                    else
                    {
                        consoleOutput += "??????????";
                    }
                }
                consoleOutput += "\n";
            }

            consoleOutput += translatedCommandsTable.GetEntry(HINT_COMMAND_FEEDBACK_ID).LocalizedValue;
            ConsoleUI.Instance.AddToHistory(consoleOutput);
        }

        void OnProgressCommand()
        {
            if (RevealableObjectHandler.Instance == null)
            {
                Debug.LogError("Progress command was typed when RevealableObjectHandler was not here");
            }

            var translatedTable = commandsTable.GetTable();
            string consoleOutput = translatedTable.GetEntry(REVEALED_ITEMS_LOCALIZATION_ID).LocalizedValue + " :";
            foreach (ObjectImportance importance in Enum.GetValues(typeof(ObjectImportance)))
            {
                List<RevealableObjectBundle> bundles = RevealableObjectHandler.Instance.GetBundlesOfImportance(importance);
                consoleOutput += "\n" + translatedTable.GetEntry(importance.ToString().ToLower()).LocalizedValue
                    + " : " + bundles.Where(i => i.IsRevealed).Count().ToString()
                    + " / " + bundles.Count();
            }

            ConsoleUI.Instance.AddToHistory(consoleOutput);
        }

        void OnHelpCommand()
        {
            ConsoleUI.Instance.AddToHistory(commandsTable.GetTable().GetEntry(HELP_COMMAND_FEEDBACK_ID).LocalizedValue);
        }

        void OnWordRevealed()
        {
            PlayerInputsHandler.Instance.ToggleWordMode();
        }

        public void OnNavigateConsoleHistory(int delta)
        {
            currentNavigationIndex = Mathf.Clamp(currentNavigationIndex + delta, 0, previousInputs.Count);
            SetInputAsHistoryIndex();
        }

        void SetInputAsHistoryIndex()
        {
            if (currentNavigationIndex >= 0 && currentNavigationIndex < previousInputs.Count)
            {
                currentWordInput = previousInputs[currentNavigationIndex];
                ConsoleUI.Instance.UpdateInput(currentWordInput);
            }
            else
            {
                ClearWord();
                ConsoleUI.Instance.UpdateInput(currentWordInput);
            }
        }

        void ResetConsoleNavigaion()
        {
            currentNavigationIndex = previousInputs.Count;
        }
    }
}
