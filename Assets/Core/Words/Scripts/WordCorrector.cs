using LW.Data;
using System;
using UnityEngine;
using UnityEngine.Localization;

namespace LW.Word
{
    public class WordCorrector : MonoBehaviour
    {
        [SerializeField] LocalizedStringTable stringTable;

        public void AttemptParsingToID(string word, Action<WordID> onSuccesfulParse, Action<string> OnFailedParse)
        {
            Debug.LogWarning("TODO : MISSING AUTOCORRECT");

            var table = stringTable.GetTable();

            foreach (WordID id in Enum.GetValues(typeof(WordID)))
            {
                string stringID = id.ToString();
                if (table.GetEntry(id.ToString()) == null)
                {
                    Debug.LogWarning("AN ID IN THE WORDID ENUM DOES NOT EXIST IN THE STRING TABLE");
                    continue;
                }

                if (ReplaceSpecialCharacterWithNormalOnes(table.GetEntry(stringID).LocalizedValue.ToLower()) == ReplaceSpecialCharacterWithNormalOnes(word.ToLower()))
                {
                    onSuccesfulParse?.Invoke(id);
                    return;
                }
            }

            OnFailedParse?.Invoke(word );
        }

        string ReplaceSpecialCharacterWithNormalOnes(string input)
        {
            input = input.Replace("�", "e");
            input = input.Replace("�", "E");
            input = input.Replace("�", "e");
            input = input.Replace("�", "E");
            input = input.Replace("�", "e");
            input = input.Replace("�", "E");
            input = input.Replace("�", "e");
            input = input.Replace("�", "E");
            input = input.Replace("�", "a");
            input = input.Replace("�", "A");
            input = input.Replace("�", "a");
            input = input.Replace("�", "A");
            input = input.Replace("�", "a");
            input = input.Replace("�", "A");
            input = input.Replace("�", "c");
            input = input.Replace("�", "C");
            input = input.Replace("�", "i");
            input = input.Replace("�", "I");
            input = input.Replace("�", "i");
            input = input.Replace("�", "I");
            input = input.Replace("�", "o");
            input = input.Replace("�", "O");
            input = input.Replace("�", "o");
            input = input.Replace("�", "O");
            // Add more replacements for other special characters as needed

            return input;
        }
    }
}