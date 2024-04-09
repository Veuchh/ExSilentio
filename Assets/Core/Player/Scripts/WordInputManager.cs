using LW.Data;
using LW.Level;
using LW.Word;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;

namespace LW.Player
{
    public class WordInputManager : MonoBehaviour
    {
        const string HELP_COMMAND_FEEDBACK_ID = "helpCommandFeedbackID";
        const string REVEALED_ITEMS_LOCALIZATION_ID = "revealedItems";
        const string CORE_LOCALIZATION_ID = "Core";
        const string SECONDARY_LOCALIZATION_ID = "Secondary";
        const string BONUS_LOCALIZATION_ID = "Bonus";

        [SerializeField] LocalizedStringTable stringTable;
        [SerializeField] WordDatabase wordDatabase;
        [SerializeField] WordCorrector wordCorrector;
        string currentWordInput;

        public void ToggleConsole(bool isToggled)
        {
            ConsoleUI.Instance.ToggleConsole(isToggled);
        }

        public void ClearWord()
        {
            currentWordInput = string.Empty;
        }

        public void AddCharacter(char chr)
        {
            if (!UsableCharacters.Characters.Contains(chr))
                return;

            currentWordInput += chr;
            ConsoleUI.Instance.UpdateInput(currentWordInput);
        }

        public void RemoveCharacter()
        {
            if (!string.IsNullOrEmpty(currentWordInput))
            {
                currentWordInput = currentWordInput.Substring(0, currentWordInput.Length - 1);
                ConsoleUI.Instance.UpdateInput(currentWordInput);
            }
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

            if (!wordCorrector.AttemptParsingToCommand(currentWordInput, OnSuccesfullCommandParse))
                wordCorrector.AttemptParsingToID(currentWordInput, OnSuccesfullParse, OnFailedParse);

            ClearWord();
            ConsoleUI.Instance.UpdateInput(currentWordInput);
        }

        private void OnSuccesfullCommandParse(CommandID id)
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
        }

        void OnProgressCommand()
        {
            if (RevealableObjectHandler.Instance == null)
            {
                Debug.LogError("Progress command was typed when not in console");
            }

            var translatedTable = stringTable.GetTable();
            string consoleOutput = translatedTable.GetEntry(REVEALED_ITEMS_LOCALIZATION_ID).LocalizedValue + " :";
            foreach (ObjectImportance importance in Enum.GetValues(typeof(ObjectImportance)))
            {
                List<RevealableObjectBundle> bundles = RevealableObjectHandler.Instance.GetBundleOfImportnce(importance);
                consoleOutput += "\n" + translatedTable.GetEntry(importance.ToString()).LocalizedValue
                    + " : " + bundles.Where(i => i.IsRevealed).Count().ToString() 
                    + " / " + bundles.Count();
            }

            ConsoleUI.Instance.AddToHistory(consoleOutput);
        }

        void OnHelpCommand()
        {
            ConsoleUI.Instance.AddToHistory(stringTable.GetTable().GetEntry(HELP_COMMAND_FEEDBACK_ID).LocalizedValue);
        }

        void OnWordRevealed()
        {
            PlayerInputsHandler.Instance.ToggleWordMode();
        }
    }
}