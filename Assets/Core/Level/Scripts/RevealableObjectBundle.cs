using LW.Data;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Level
{
    public class RevealableObjectBundle : MonoBehaviour
    {
        [SerializeField] List<RevealableItem> itemsRevealed;
        [SerializeField] WordID awaitedID;
        [SerializeField] ObjectImportance objectImportance;
        
        bool isRevealed = false;
        WordDatabaseEntry entry;

        public WordID ID => awaitedID;
        public ObjectImportance ObjectImportance => objectImportance;
        public bool IsRevealed => isRevealed;
        public WordDatabaseEntry Entry => entry;

        private void Start()
        {
            RevealableObjectHandler.Instance.RegisterBundle(this);
        }

        public void RevealBundle(WordID usedID)
        {
            foreach (RevealableItem revealableItem in itemsRevealed)
            {
                //if the item id is the same as the expected but the used ID is different from the expect one, we change it
                if (revealableItem.ID == awaitedID)
                {
                    revealableItem.UpdateID(usedID);
                }
                revealableItem.RevealItem();
            }

            isRevealed = true;
        }

        [Button]
        void SetupElements()
        {
            itemsRevealed = new List<RevealableItem>();

            foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
            {
                RevealableItem reveleableItem = renderer.GetComponent<RevealableItem>();

                if (reveleableItem == null)
                {
                    reveleableItem = renderer.gameObject.AddComponent<RevealableItem>();
                    reveleableItem.Init(awaitedID, renderer);
                }

                itemsRevealed.Add(reveleableItem);
            }
        }

        public void AssignEntry(WordDatabaseEntry assignedEntry)
        {
            entry = assignedEntry;
        }
    }
}