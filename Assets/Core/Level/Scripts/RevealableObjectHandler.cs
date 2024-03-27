using LW.Data;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Level
{
    public class RevealableObjectHandler : MonoBehaviour
    {
        public static RevealableObjectHandler Instance { get; private set; }

        [SerializeField] WordDatabase database;

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

        public void AttemptWordDiscovery(DatabaseQueryResult queryResult)
        {
            RevealableObjectBundle bundleToReveal = null;

            WordID usedID = 0;
            ObjectImportance currentImportance = ObjectImportance.Bonus;

            bool isSemanticallyClose = false;

            foreach (RevealableObjectBundle bundle in bundles)
            {
                if (bundle.IsRevealed)
                    continue;

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

            if (bundleToReveal != null)
            {
                bundleToReveal.RevealBundle(usedID);
                //Console feedback
                ConsoleUI.Instance.AddToHistory($"{queryResult.MainResult.ID.ToString()} was revealed.");
            }
            else if (isSemanticallyClose)
            {
                ConsoleUI.Instance.AddToHistory($"{queryResult.MainResult.ID.ToString()} was semantically close to a word in the scene.");
            }
        }
    }
}