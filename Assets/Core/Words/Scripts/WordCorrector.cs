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
            input = input.Replace("é", "e");
            input = input.Replace("É", "E");
            input = input.Replace("è", "e");
            input = input.Replace("È", "E");
            input = input.Replace("ê", "e");
            input = input.Replace("Ê", "E");
            input = input.Replace("ë", "e");
            input = input.Replace("Ë", "E");
            input = input.Replace("à", "a");
            input = input.Replace("À", "A");
            input = input.Replace("â", "a");
            input = input.Replace("Â", "A");
            input = input.Replace("ä", "a");
            input = input.Replace("Ä", "A");
            input = input.Replace("ç", "c");
            input = input.Replace("Ç", "C");
            input = input.Replace("ï", "i");
            input = input.Replace("Ï", "I");
            input = input.Replace("î", "i");
            input = input.Replace("Î", "I");
            input = input.Replace("ö", "o");
            input = input.Replace("Ö", "O");
            input = input.Replace("ô", "o");
            input = input.Replace("Ô", "O");
            // Add more replacements for other special characters as needed

            return input;
        }
    }
}