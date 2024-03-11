using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LW.Data;
using System.Linq;
using LW.Word;

namespace LW.Player
{
    public class WordInputManager : MonoBehaviour
    {
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