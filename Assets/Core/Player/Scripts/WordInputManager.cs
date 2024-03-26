using LW.Data;
using LW.Word;
using System.Linq;
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
            currentWordInput = "";
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
            if (currentWordInput.Length > 0)
            {
                currentWordInput.Remove(currentWordInput.Length - 1);
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

            var queryResult = wordDatabase.AttemptDatabaseRetrieve(wordID);

            RevealableObjectHandler.Instance.AttemptWordDiscovery(queryResult);
        }

        private void OnFailedParse()
        {
            Debug.Log($"Parse failed");
        }

        public void SubmitWord()
        {
            ConsoleUI.Instance.AddToHistory(currentWordInput);

            if (string.IsNullOrEmpty(currentWordInput))
                return;

            wordCorrector.AttemptParsingToID(currentWordInput, OnSuccesfullParse, OnFailedParse);
            ClearWord();
            ConsoleUI.Instance.UpdateInput(currentWordInput);
        }
    }
}