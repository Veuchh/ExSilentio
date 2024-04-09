using UnityEngine;
using TMPro;
using NaughtyAttributes;
using UnityEngine.UI;

namespace LW.UI
{
    public class ConsoleUI : MonoBehaviour
    {
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] ScrollRect scrollRect;
        [SerializeField] VerticalLayoutGroup historyParent;
        [SerializeField] TextMeshProUGUI input;
        [SerializeField] ConsoleEntry consoleEntryPrefab;

        [SerializeField] bool separateWithDashes = false;
        [SerializeField] bool changeBackground = false;
        [SerializeField, ShowIf(nameof(changeBackground))] Color backgroundColor1;
        [SerializeField, ShowIf(nameof(changeBackground))] Color backgroundColor2;
        [SerializeField] bool changeTextColor = true;
        [SerializeField, ShowIf(nameof(changeTextColor))] Color textColor1;
        [SerializeField, ShowIf(nameof(changeTextColor))] Color textColor2;

        bool isEntryEven = true;

        public static ConsoleUI Instance;
        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void ToggleConsole(bool toggle)
        {
            canvasGroup.alpha = toggle ? 1 : 0;
        }

        public void AddToHistory(string newHistory)
        {
            if (separateWithDashes)
                Instantiate(consoleEntryPrefab, historyParent.transform).UpdateText("------------------------------------");

            ConsoleEntry newEntry = Instantiate(consoleEntryPrefab, historyParent.transform);
            newEntry.UpdateText(newHistory);

            if (changeTextColor)
                newEntry.UpdateTextColor(isEntryEven ? textColor2 : textColor1);

            if (changeBackground)
                newEntry.UpdateBackgroundColor(isEntryEven ? backgroundColor2 : backgroundColor1);

            isEntryEven = !isEntryEven;

            Canvas.ForceUpdateCanvases();

            historyParent.CalculateLayoutInputVertical();
            historyParent.GetComponent<ContentSizeFitter>().SetLayoutVertical(); 
            
            scrollRect.verticalNormalizedPosition = 0;
        }

        public void UpdateInput(string newInput)
        {
            input.text = newInput;
        }
    }
}