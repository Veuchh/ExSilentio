using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LW.Data;
using System.Linq;

namespace LW.Player
{
    public class WordInputManager : MonoBehaviour
    {
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

        public void SubmitWord()
        {
            ConsoleUI.Instance.AddToHistory(currentWordInput);
            ClearWord();
            ConsoleUI.Instance.UpdateInput(currentWordInput);
        }
    }
}