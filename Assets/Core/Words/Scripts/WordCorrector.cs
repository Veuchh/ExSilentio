using Codice.CM.Common;
using LW.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

namespace LW.Word
{
    public class WordCorrector : MonoBehaviour
    {
        [SerializeField] LocalizedStringTable stringTable;

        public void AttemptParsingToID(string word, Action<WordID> onSuccesfulParse, Action OnFailedParse)
        {
            Debug.LogWarning("TODO : MISSING AUTOCORRECT");

            Locale currentLocale = LocalizationSettings.SelectedLocale;
            //Debug.Log(stringTable.GetTable());
            foreach (WordID id in System.Enum.GetValues(typeof(WordID)))
            {
                if (stringTable.GetTable().GetEntry(id.ToString()) == null)
                {
                    Debug.LogError("AN ID IN THE WORDID ENUM DOES NOT EXIST IN THE STRING TABLE");
                    continue;
                }
               
                if (stringTable.GetTable().GetEntry(id.ToString()).LocalizedValue.ToLower() == word.ToLower())
                {
                    onSuccesfulParse?.Invoke(id);
                    return;
                }
            }

            OnFailedParse?.Invoke();
        }
    }
}