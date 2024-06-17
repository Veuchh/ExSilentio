using LW.Data;
using System;
using UnityEngine;
using UnityEngine.Localization;

namespace LW.Word
{
    public class WordCorrector : MonoBehaviour
    {
        [SerializeField] LocalizedStringTable wordStringTable;
        [SerializeField] LocalizedStringTable commandStringTable;

        public bool AttemptParsingToCommand(string currentWordInput, Action<CommandID, string> onSuccesfullCommandParse)
        {
            var table = commandStringTable.GetTable();

            CommandID bestCandidate = (CommandID)(-1);
            int bestCandidateScore = int.MaxValue;

            string[] separatedInput = currentWordInput.Split(" ");
            int firstWordID = 0;

            for (int i = 0; i < separatedInput.Length; i++)
            {
                if (!string.IsNullOrEmpty(separatedInput[i]))
                {
                    firstWordID = i;
                    break;
                }
            }

            string arguments = string.Empty;

            for (int i = firstWordID + 1; i < separatedInput.Length; i++)
            {
                arguments += separatedInput[i] + (i == separatedInput.Length - 1 ? "" : " ");
            }

            if (arguments.Length > 0)
                arguments.Remove(arguments.Length - 1);

            foreach (CommandID id in Enum.GetValues(typeof(CommandID)))
            {
                string stringID = id.ToString();

                if (table.GetEntry(id.ToString()) == null)
                {
                    Debug.LogWarning("AN ID IN THE COMMANDID ENUM DOES NOT EXIST IN THE STRING TABLE");
                    continue;
                }

                if (NormalizeString(table.GetEntry(stringID).LocalizedValue) == NormalizeString(separatedInput[firstWordID]))
                {
                    onSuccesfullCommandParse?.Invoke(id, arguments);
                    return true;
                }
                else
                {
                    if (id == CommandID.spinboi || id == CommandID.setSpeed)
                        continue;

                    int levensteinDistance = LevensteinDistance.Calculate(NormalizeString(table.GetEntry(stringID).LocalizedValue), NormalizeString(separatedInput[firstWordID]));

                    if (levensteinDistance < bestCandidateScore)
                    {
                        bestCandidateScore = levensteinDistance;
                        bestCandidate = id;
                    }
                }
            }

            if (bestCandidateScore <= Mathf.Max(1, currentWordInput.Length / 5))
            {
                onSuccesfullCommandParse?.Invoke(bestCandidate, arguments);
                return true;
            }

            return false;
        }

        public void AttemptParsingToID(string word, Action<WordID> onSuccesfulParse, Action<string> OnFailedParse)
        {
            var table = wordStringTable.GetTable();

            WordID bestCandidate = (WordID)(-1);
            int bestCandidateScore = int.MaxValue;

            foreach (WordID id in Enum.GetValues(typeof(WordID)))
            {
                string stringID = id.ToString();
                if (table.GetEntry(id.ToString()) == null)
                {
                    Debug.LogWarning("AN ID IN THE WORDID ENUM DOES NOT EXIST IN THE STRING TABLE");
                    continue;
                }

                if (NormalizeString(table.GetEntry(stringID).LocalizedValue) == NormalizeString(word))
                {
                    onSuccesfulParse?.Invoke(id);
                    return;
                }
                else
                {
                    int levensteinDistance = LevensteinDistance.Calculate(NormalizeString(table.GetEntry(stringID).LocalizedValue), NormalizeString(word));

                    if (levensteinDistance < bestCandidateScore)
                    {
                        bestCandidateScore = levensteinDistance;
                        bestCandidate = id;
                    }
                }
            }

            if (bestCandidateScore <= word.Length / 3)
            {
                onSuccesfulParse?.Invoke(bestCandidate);
                return;
            }

            OnFailedParse?.Invoke(word);
        }

        string NormalizeString(string input)
        {
            input = input.ToLower();

            input = input.Replace("é", "e");
            input = input.Replace("É", "E");
            input = input.Replace("è", "e");
            input = input.Replace("È", "E");
            input = input.Replace("ê", "e");
            input = input.Replace("Ê", "E");
            input = input.Replace("ë", "e");
            input = input.Replace("Ë", "E");
            input = input.Replace("à", "a");
            input = input.Replace("À", "A");
            input = input.Replace("â", "a");
            input = input.Replace("Â", "A");
            input = input.Replace("ä", "a");
            input = input.Replace("Ä", "A");
            input = input.Replace("ç", "c");
            input = input.Replace("Ç", "C");
            input = input.Replace("ï", "i");
            input = input.Replace("Ï", "I");
            input = input.Replace("î", "i");
            input = input.Replace("Î", "I");
            input = input.Replace("ö", "o");
            input = input.Replace("Ö", "O");
            input = input.Replace("ô", "o");
            input = input.Replace("Ô", "O");
            // Add more replacements for other special characters as needed

            return input;
        }
    }
}