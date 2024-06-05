using LW.Data;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Button playButton;
    [SerializeField] Button optionsButton;
    [SerializeField] Button quitButton;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] bool isPauseMenu;

    public static event Action onMainMenuPressPlay;
    public static event Action onPauseMenuPressPlay;

    public static MainMenu Instance;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
        playButton.onClick.AddListener(OnPlayButton);
        optionsButton.onClick.AddListener(OnOptionsButton);
        quitButton.onClick.AddListener(OnQuitButton);

        ToggleMenu(!isPauseMenu);
    }

    private void OnDestroy()
    {
        playButton.onClick.RemoveAllListeners();
        optionsButton.onClick.RemoveAllListeners();
        quitButton.onClick.RemoveAllListeners();
    }

    public void ToggleMenu(bool isToggled)
    {
        canvasGroup.alpha = isToggled ? 1 : 0;
        canvasGroup.interactable = isToggled;
        canvasGroup.blocksRaycasts = isToggled;
        StaticData.OpenWindowsAmount += isToggled ? 1 : -1;
    }

    void OnPlayButton()
    {
        ToggleMenu(false);

        if (isPauseMenu)
        {
            onPauseMenuPressPlay?.Invoke();
        }
        else
        {
            isPauseMenu = true;
            onMainMenuPressPlay?.Invoke();
        }
    }

    void OnOptionsButton()
    {
        Debug.LogWarning("TODO : Add options menu");
    }

    void OnQuitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
