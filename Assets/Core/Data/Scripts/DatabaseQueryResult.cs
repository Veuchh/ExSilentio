using System.Collections.Generic;

namespace LW.Data
{
    public struct DatabaseQueryResult
    {
        private WordDatabaseEntry mainResult;
        private List<WordDatabaseEntry> secondaryResults;
        private List<WordDatabaseEntry> semanticallyCloseIDs;

        public WordDatabaseEntry MainResult { get { return mainResult; } }
        public List<WordDatabaseEntry> SecondaryResults { get { return secondaryResults; } }

        public List<WordDatabaseEntry> SemanticallyCloseIDs { get => semanticallyCloseIDs; set => semanticallyCloseIDs = value; }

        public DatabaseQueryResult(WordDatabaseEntry mainResult, List<WordDatabaseEntry> secondaryResults, List<WordDatabaseEntry> semanticallyCloseIDs)
        {
            this.mainResult = mainResult;
            this.secondaryResults = secondaryResults;
            this.semanticallyCloseIDs = semanticallyCloseIDs;
        }
    }
}