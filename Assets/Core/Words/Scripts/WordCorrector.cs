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

                if (table.GetEntry(stringID).LocalizedValue.ToLower() == word.ToLower())
                {
                    onSuccesfulParse?.Invoke(id);
                    return;
                }
            }

            OnFailedParse?.Invoke(word );
        }
    }
}