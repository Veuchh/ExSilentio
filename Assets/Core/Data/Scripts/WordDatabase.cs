using System;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Data
{
    [CreateAssetMenu(fileName = "WordDatabase_", menuName = "ScriptableObject/Word Database")]
    public class WordDatabase : ScriptableObject
    {
        [SerializeField] List<WordDatabaseEntry> database;

        public DatabaseQueryResult AttemptDatabaseRetrieve(WordID attemptedID)
        {
            WordDatabaseEntry mainResult = null;
            List<WordDatabaseEntry> secondaryResult = new List<WordDatabaseEntry>();
            List<WordDatabaseEntry> semanticallyClose = new List<WordDatabaseEntry>();

            foreach (WordDatabaseEntry entry in database)
            {
                if (entry.ID == attemptedID)
                {
                    mainResult = entry;
                }

                if (entry.AdditionalAcceptedIds.Contains(attemptedID))
                {
                    secondaryResult.Add(entry);
                }

                if (entry.CloselySemanticIds.Contains(attemptedID))
                {
                    semanticallyClose.Add(entry);
                }
            }

            return new DatabaseQueryResult(mainResult, secondaryResult, semanticallyClose);
        }

        public List<WordDatabaseEntry> GetDatabase()
        {
            return database;
        }

        public void UpdateEntry(WordDatabaseEntry newEntry)
        {
            foreach (WordDatabaseEntry entry in database)
            {
                if (entry.ID == newEntry.ID)
                {
                    entry.AdditionalAcceptedIds = newEntry.AdditionalAcceptedIds;
                    entry.CloselySemanticIds = newEntry.CloselySemanticIds;
                    entry.GdNotes = newEntry.GdNotes;
                }
            }
        }

        public void AddEntry(WordDatabaseEntry newEntry)
        {
            database.Add(newEntry);
        }

        public void RemoveEntryByID(WordID idToRemove)
        {
            for (int i = database.Count - 1; i >= 0; i--)
            {
                if (database[i].ID == idToRemove)
                {
                    database.RemoveAt(i);
                }
            }
        }
    }
}