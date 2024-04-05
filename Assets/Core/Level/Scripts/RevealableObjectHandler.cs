using LW.Data;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace LW.Level
{
    public class RevealableObjectHandler : MonoBehaviour
    {
        public static string REVEAL_FAILED_FEEDBACK_TRANSLATION_KEY = "revealFailedFeedback";
        public static string REVEAL_SUCCESFUL_FEEDBACK_TRANSLATION_KEY = "revealSuccesfulFeedback";
        public static string REVEAL_ALREADY_HERE_FEEDBACK_TRANSLATION_KEY = "revealAlreadyRevealedFeedback";
        public static string REVEAL_CLOSE_FEEDBACK_TRANSLATION_KEY = "revealSemanticallyCloseFeedback";
        public static RevealableObjectHandler Instance { get; private set; }

        [SerializeField] WordDatabase database;
        [SerializeField] LocalizedStringTable stringTable;

        List<RevealableObjectBundle> bundles = new List<RevealableObjectBundle>();

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        public void RegisterBundle(RevealableObjectBundle newBundle)
        {
            if (!bundles.Contains(newBundle))
            {
                bundles.Add(newBundle);
                newBundle.AssignEntry(database.GetEntry(newBundle.ID));
            }
            else
            {
                Debug.LogWarning("A bundle attempted to register twice. Register attempt rejected.");
            }
        }

        public void AttemptWordDiscovery(DatabaseQueryResult queryResult, Action OnSuccesfulWordReveal)
        {
            RevealableObjectBundle bundleToReveal = null;

            WordID usedID = 0;
            ObjectImportance currentImportance = ObjectImportance.Bonus;

            bool isSemanticallyClose = false;
            bool isAlreadyRevealed = false;
            foreach (RevealableObjectBundle bundle in bundles)
            {
                if (bundle.IsRevealed)
                {
                    if (bundle.ID == queryResult.MainResult.ID)
                        isAlreadyRevealed = true;

                    continue;
                }

                //If we found the specific word we are looking for, no need to look into things further
                if (bundle.ID == queryResult.MainResult.ID)
                {
                    bundleToReveal = bundle;
                    usedID = queryResult.MainResult.ID;
                    break;
                }

                //We cycle through the specific IDs.
                //If a word has a higher priority, we display it.
                foreach (WordDatabaseEntry secondaryResult in queryResult.SecondaryResults)
                {
                    //if it is matching
                    if (bundle.ID == secondaryResult.ID)
                    {
                        //if the item is of the same importance or more important
                        if ((int)currentImportance >= (int)bundle.ObjectImportance)
                        {
                            bundleToReveal = bundle;
                            usedID = secondaryResult.ID;
                            currentImportance = bundle.ObjectImportance;
                        }
                    }
                }

                if (bundleToReveal == null && isSemanticallyClose == false)
                {
                    foreach (WordDatabaseEntry semanticallyCloseEntrie in queryResult.SemanticallyCloseIDs)
                    {
                        if (bundle.ID == semanticallyCloseEntrie.ID)
                        {
                            isSemanticallyClose = true;
                            break;
                        }
                    }
                }
            }

            string consoleFeedback = string.Empty;
            string translatedID = stringTable.GetTable().GetEntry(queryResult.MainResult.ID.ToString()).LocalizedValue;

            if (bundleToReveal != null)
            {
                OnSuccesfulWordReveal?.Invoke();
                bundleToReveal.RevealBundle(usedID);
                consoleFeedback += " " + stringTable.GetTable().GetEntry(REVEAL_SUCCESFUL_FEEDBACK_TRANSLATION_KEY).LocalizedValue;
                translatedID = "[!] " + translatedID;
            }
            else if (isSemanticallyClose)
            {
                consoleFeedback = stringTable.GetTable().GetEntry(REVEAL_CLOSE_FEEDBACK_TRANSLATION_KEY).LocalizedValue;
                translatedID = "[?] " + translatedID;
            }
            else if (isAlreadyRevealed)
            {
                consoleFeedback += " " + stringTable.GetTable().GetEntry(REVEAL_ALREADY_HERE_FEEDBACK_TRANSLATION_KEY).LocalizedValue;
                translatedID = "[.] " + translatedID;
            }
            else
            {
                consoleFeedback += " " + stringTable.GetTable().GetEntry(REVEAL_FAILED_FEEDBACK_TRANSLATION_KEY).LocalizedValue;
                translatedID = "[...]" + translatedID;
            }

            ConsoleUI.Instance.AddToHistory($"{translatedID}{consoleFeedback}");
        }

        public void OnParseFailed(string failedToParseString)
        {
            string revealFailedEntry = stringTable.GetTable().GetEntry(REVEAL_FAILED_FEEDBACK_TRANSLATION_KEY).LocalizedValue;
            ConsoleUI.Instance.AddToHistory($"[...] {failedToParseString} {revealFailedEntry}");
        }
    }
}