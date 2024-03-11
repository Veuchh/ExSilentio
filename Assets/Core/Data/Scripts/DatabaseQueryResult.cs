using System.Collections.Generic;

namespace LW.Data
{
    public struct DatabaseQueryResult
    {
        private WordDatabaseEntry mainResult;
        private List<WordDatabaseEntry> secondaryResults;

        public WordDatabaseEntry MainResult { get { return mainResult; } }
        public List<WordDatabaseEntry> SecondaryResults { get { return secondaryResults; } }

        public DatabaseQueryResult(WordDatabaseEntry mainResult, List<WordDatabaseEntry> secondaryResults)
        {
            this.mainResult = mainResult;
            this.secondaryResults = secondaryResults;
        }
    }
}