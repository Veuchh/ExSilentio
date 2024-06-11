using LW.Audio;
using LW.Data;
using LW.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

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
        [SerializeField] HintDatabase hintDatabase;
        [SerializeField] LocalizedStringTable stringTable;
        [SerializeField] bool countsTowardsTotalLevelToGuess = true;


        [Header("Wwise Events")]
        [SerializeField] AK.Wwise.Event uiClGoodWord;
        [SerializeField] AK.Wwise.Event uiClWrongWord;

        List<RevealableObjectBundle> bundles = new List<RevealableObjectBundle>();

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        public void RegisterBundle(RevealableObjectBundle newBundle, bool revealByDefault)
        {
            if (!bundles.Contains(newBundle))
            {
                bundles.Add(newBundle);
                newBundle.Init(database.GetEntry(newBundle.ID));

                //reveal the bundle if it is saved
                string savedWord = SaveLoad.GetWordFromPlayerPrefs(newBundle.GetBundleKey);

                if (revealByDefault)
                {
                    WordID id = newBundle.ID;
                    newBundle.RevealBundle(id, stringTable, false);
                }
                else if (!string.IsNullOrEmpty(savedWord))
                {
                    WordID id = (WordID)Enum.Parse(typeof(WordID), savedWord);
                    newBundle.RevealBundle(id, stringTable, false);
                }
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

                    else
                        foreach (WordDatabaseEntry entry in queryResult.SecondaryResults)
                            if (entry.ID == bundle.ID)
                            {
                                isAlreadyRevealed = true;
                                break;
                            }

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
                            usedID = queryResult.MainResult.ID;
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
                consoleFeedback += " " + stringTable.GetTable().GetEntry(REVEAL_SUCCESFUL_FEEDBACK_TRANSLATION_KEY).LocalizedValue;
                bundleToReveal.RevealBundle(usedID, stringTable);

                translatedID = "[!] " + translatedID;

                //checking if all core objects were discovered
                if (countsTowardsTotalLevelToGuess && bundleToReveal.ObjectImportance == ObjectImportance.Core)
                {
                    bool allCoreRevealed = true;

                    foreach (var item in GetBundlesOfImportance(ObjectImportance.Core))
                    {
                        if (!item.IsRevealed)
                        {
                            allCoreRevealed = false;
                            break;
                        }
                    }

                    if (allCoreRevealed)
                    {
                        SaveLoad.SaveIntToPlayerPrefs(
                            SaveLoad.SCENE_WITH_ALL_CORE_REVEALED,
                            SaveLoad.GetIntFromPlayerPrefs(SaveLoad.SCENE_WITH_ALL_CORE_REVEALED) + 1);
                    }
                }

                WwiseInterface.Instance.PlayEvent(uiClGoodWord);
            }
            else if (isSemanticallyClose)
            {
                consoleFeedback = stringTable.GetTable().GetEntry(REVEAL_CLOSE_FEEDBACK_TRANSLATION_KEY).LocalizedValue;
                translatedID = "[?] " + translatedID;
                WwiseInterface.Instance.PlayEvent(uiClWrongWord);
            }
            else if (isAlreadyRevealed)
            {
                consoleFeedback += " " + stringTable.GetTable().GetEntry(REVEAL_ALREADY_HERE_FEEDBACK_TRANSLATION_KEY).LocalizedValue;
                translatedID = "[.] " + translatedID;
                WwiseInterface.Instance.PlayEvent(uiClWrongWord);
            }
            else
            {
                consoleFeedback += " " + stringTable.GetTable().GetEntry(REVEAL_FAILED_FEEDBACK_TRANSLATION_KEY).LocalizedValue;
                translatedID = "[...]" + translatedID;
                WwiseInterface.Instance.PlayEvent(uiClWrongWord);
            }

            ConsoleUI.Instance.AddToHistory($"{translatedID}{consoleFeedback}");
        }

        public List<RevealableObjectBundle> GetBundlesOfImportance(ObjectImportance objectImportance)
        {
            List<RevealableObjectBundle> bundles = new List<RevealableObjectBundle>();

            foreach (var bundle in this.bundles)
            {
                if (bundle.ObjectImportance == objectImportance)
                {
                    bundles.Add(bundle);
                }
            }

            return bundles;
        }

        public List<RevealableObjectBundle> GetBundles() => bundles;

        public void OnParseFailed(string failedToParseString)
        {
            string revealFailedEntry = stringTable.GetTable().GetEntry(REVEAL_FAILED_FEEDBACK_TRANSLATION_KEY).LocalizedValue;
            ConsoleUI.Instance.AddToHistory($"[...] {failedToParseString} {revealFailedEntry}");
            WwiseInterface.Instance.PlayEvent(uiClWrongWord);
        }

        public void RevealAllItemsOfType(ObjectImportance importance)
        {
            foreach (RevealableObjectBundle bundle in bundles)
                if (!bundle.IsRevealed && bundle.ObjectImportance == importance)
                    bundle.RevealBundle(bundle.ID, stringTable);
        }

        internal void RevealSavedBundles()
        {
            throw new NotImplementedException();
        }
    }
}