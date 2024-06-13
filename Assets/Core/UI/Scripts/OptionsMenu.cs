using LW.Data;
using LW.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [Header("Basic Settings")]
    [SerializeField] Canvas canvas;
    [SerializeField] CanvasGroup optionsCanvasGroup;
    [SerializeField] RebindSaveLoad rebindSaveLoad;

    [Header("Buttons")]
    [SerializeField] Button returnButton;
    [SerializeField] Button accessibilityButton;
    [SerializeField] Button controlsButton;
    [SerializeField] Button graphicsButton;
    [SerializeField] Button audioButton;
    [SerializeField] Button resetButton;

    [Header("Pannels")]
    [SerializeField] CanvasGroup acessibilityPannel;
    [SerializeField] CanvasGroup controlsPannel;
    [SerializeField] CanvasGroup graphicsPannel;
    [SerializeField] CanvasGroup audioPannel;
    [SerializeField] CanvasGroup resetPannel;
    
    [Space]
    [SerializeField] List<OptionBase> options;

    public static OptionsMenu Instance;

    private void Awake()
    {
        Instance = this;

        TogglePannel(acessibilityPannel);

        returnButton.onClick.AddListener(() => ToggleMenu(false));
        accessibilityButton.onClick.AddListener(() => TogglePannel(acessibilityPannel));
        controlsButton.onClick.AddListener(() => TogglePannel(controlsPannel));
        graphicsButton.onClick.AddListener(() => TogglePannel(graphicsPannel));
        audioButton.onClick.AddListener(() => TogglePannel(audioPannel));
        resetButton.onClick.AddListener(() => TogglePannel(resetPannel));
    }

    private void Start()
    {
        rebindSaveLoad.LoadRebinds();
        //Load settings
        //Apply settings
    }

    private void OnDestroy()
    {
        returnButton.onClick.RemoveAllListeners();
        accessibilityButton.onClick.RemoveAllListeners();
        controlsButton.onClick.RemoveAllListeners();
        graphicsButton.onClick.RemoveAllListeners();
        audioButton.onClick.RemoveAllListeners();
        resetButton.onClick.RemoveAllListeners();
    }

    public void ToggleMenu()
    {
        ToggleMenu(!optionsCanvasGroup.interactable);
    }

    public void ToggleMenu(bool isToggled)
    {
        optionsCanvasGroup.alpha = isToggled ? 1 : 0;
        optionsCanvasGroup.interactable = isToggled;
        optionsCanvasGroup.blocksRaycasts = isToggled;
        StaticData.OpenWindowsAmount += isToggled ? 1 : -1;
        canvas.sortingOrder = isToggled ? StaticData.OpenWindowsAmount : -1;

        Action rebindAction = isToggled ? rebindSaveLoad.LoadRebinds : rebindSaveLoad.SaveRebinds;
        rebindAction?.Invoke();
    }

    #region ButtonCallbacks

    void TogglePannel(CanvasGroup pannelToToggle)
    {
        acessibilityPannel.alpha = 0;
        controlsPannel.alpha = 0;
        graphicsPannel.alpha = 0;
        audioPannel.alpha = 0;
        resetPannel.alpha = 0;

        acessibilityPannel.interactable = false;
        controlsPannel.interactable = false;
        graphicsPannel.interactable = false;
        audioPannel.interactable = false;
        resetPannel.interactable = false;

        acessibilityPannel.blocksRaycasts = false;
        controlsPannel.blocksRaycasts = false;
        graphicsPannel.blocksRaycasts = false;
        audioPannel.blocksRaycasts = false;
        resetPannel.blocksRaycasts = false;

        pannelToToggle.alpha = 1;
        pannelToToggle.interactable = true;
        pannelToToggle.blocksRaycasts = true;
    }

    #endregion ButtonCallbacks
}
