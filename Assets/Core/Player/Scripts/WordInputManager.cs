using LW.Data;
using LW.Level;
using LW.Word;
using System.Linq;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;

namespace LW.Player
{
    public class WordInputManager : MonoBehaviour
    {
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

            RevealableObjectHandler.Instance.AttemptWordDiscovery(queryResult);
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

            wordCorrector.AttemptParsingToID(currentWordInput, OnSuccesfullParse, OnFailedParse);
            ClearWord();
            ConsoleUI.Instance.UpdateInput(currentWordInput);
        }
    }
}