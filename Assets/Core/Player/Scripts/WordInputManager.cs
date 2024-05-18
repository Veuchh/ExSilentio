using LW.Audio;
using LW.Data;
using LW.Level;
using LW.Logger;
using LW.UI;
using LW.Word;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

namespace LW.Player
{
    public class WordInputManager : MonoBehaviour
    {
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

        private void OnUseCommand(CommandID id)
        {
            switch (id)
            {
                case CommandID.hint:
                    OnHintCommand();
                    break;
                case CommandID.progress:
                    OnProgressCommand();
                    break;
                case CommandID.help:
                    OnHelpCommand();
                    break;
            }
        }

        void OnHintCommand()
        {
            var translatedWordsTable = wordsTable.GetTable();
            var translatedCommandsTable = commandsTable.GetTable();

            string consoleOutput = translatedCommandsTable.GetEntry(HINT_COMMAND_FEEDBACK_ID).LocalizedValue;

            foreach (RevealableObjectBundle bundle in RevealableObjectHandler.Instance.GetBundles())
            {
                consoleOutput += "\n" + 
                    (bundle.IsRevealed ?
                    translatedWordsTable.GetEntry(bundle.ID.ToString()).LocalizedValue
                    : " ?????????")
                    + " : ";
                for (int hintIndex = 0; hintIndex < bundle.Hints.Count; hintIndex++)
                {
                    consoleOutput += $"\n\t{hintIndex}.";
                    string hintKey = bundle.Entry.HintsID[hintIndex];
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
            }

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