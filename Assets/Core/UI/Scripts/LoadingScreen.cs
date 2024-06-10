using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Instance;

    [SerializeField] Canvas canvas;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Image mask;
    [SerializeField] TextMeshProUGUI backgroundText;
    [SerializeField] TextMeshProUGUI foregroundText;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }

    public void ToggleLoadingScreen(bool toggle, string levelName = "")
    {
        mask.fillAmount = 0;
        canvas.sortingOrder = toggle ? 2 : -1;
        canvasGroup.alpha = toggle ? 1 : 0;

        levelName = levelName.ToUpper();

        backgroundText.text = levelName;
        foregroundText.text = levelName;
    }

    public void UpdateProgress(float loadingRatio)
    {
        mask.fillAmount = loadingRatio;
    }
}
