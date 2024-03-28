using UnityEngine;
using TMPro;

public class ConsoleUI : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] TextMeshProUGUI history;
    [SerializeField] TextMeshProUGUI input;

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
        history.text += newHistory + "\n";
    }

    public void UpdateInput(string newInput)
    {
        input.text = newInput;
    }
}
