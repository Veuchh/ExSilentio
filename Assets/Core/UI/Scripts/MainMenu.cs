using LW.Audio;
using LW.Data;
using LW.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] Button playButton;
    [SerializeField] Button optionsButton;
    [SerializeField] Button quitButton;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] bool isPauseMenu;
    [SerializeField] AK.Wwise.Event toggleMenuOn;
    [SerializeField] AK.Wwise.Event toggleMenuOff;

    public static event Action onMainMenuPressPlay;
    public static event Action onPauseMenuPressPlay;

    public static MainMenu Instance;

    public bool IsPauseMenu => isPauseMenu;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
        playButton.onClick.AddListener(OnPlayButton);
        optionsButton.onClick.AddListener(OnOptionsButton);
        quitButton.onClick.AddListener(OnQuitButton);

        ToggleMenu(!IsPauseMenu);

        if (isPauseMenu)
        {
            StaticData.OpenWindowsAmount = 0;
        }
    }

    private void OnDestroy()
    {
        playButton.onClick.RemoveAllListeners();
        optionsButton.onClick.RemoveAllListeners();
        quitButton.onClick.RemoveAllListeners();
    }

    public void ToggleMenu()
    {
        ToggleMenu(!canvasGroup.interactable);
    }

    public void ToggleMenu(bool isToggled)
    {
        canvas.sortingOrder = isToggled ? 1 : -1;
        canvasGroup.alpha = isToggled ? 1 : 0;
        canvasGroup.interactable = isToggled;
        canvasGroup.blocksRaycasts = isToggled;
        StaticData.OpenWindowsAmount += isToggled ? 1 : -1;
        WwiseInterface.Instance.PlayEvent(isToggled ? toggleMenuOn : toggleMenuOff);
    }

    void OnPlayButton()
    {
        ToggleMenu(false);

        if (IsPauseMenu)
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
        OptionsMenu.Instance.ToggleMenu(true, isPauseMenu);
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
