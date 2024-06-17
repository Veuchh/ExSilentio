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

            input = input.Replace("�", "e");
            input = input.Replace("�", "E");
            input = input.Replace("�", "e");
            input = input.Replace("�", "E");
            input = input.Replace("�", "e");
            input = input.Replace("�", "E");
            input = input.Replace("�", "e");
            input = input.Replace("�", "E");
            input = input.Replace("�", "a");
            input = input.Replace("�", "A");
            input = input.Replace("�", "a");
            input = input.Replace("�", "A");
            input = input.Replace("�", "a");
            input = input.Replace("�", "A");
            input = input.Replace("�", "c");
            input = input.Replace("�", "C");
            input = input.Replace("�", "i");
            input = input.Replace("�", "I");
            input = input.Replace("�", "i");
            input = input.Replace("�", "I");
            input = input.Replace("�", "o");
            input = input.Replace("�", "O");
            input = input.Replace("�", "o");
            input = input.Replace("�", "O");
            // Add more replacements for other special characters as needed

            return input;
        }
    }
}