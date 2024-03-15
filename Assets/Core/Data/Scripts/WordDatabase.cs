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
            }

            return new DatabaseQueryResult(mainResult, secondaryResult);
        }
    }
}