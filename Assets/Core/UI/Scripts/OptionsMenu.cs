using LW.Data;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LW.UI
{
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
        [SerializeField] Button resetCategoryButton;
        [SerializeField] Button resetButton;

        [Header("ButtonsColor")]
        [SerializeField] Color defaultButtonColor;
        [SerializeField] Color highlightedButtonColor;
        [SerializeField] Color clickedButtonColor;

        [Header("Pannels")]
        [SerializeField] CanvasGroup acessibilityPannel;
        [SerializeField] CanvasGroup controlsPannel;
        [SerializeField] CanvasGroup graphicsPannel;
        [SerializeField] CanvasGroup audioPannel;
        [SerializeField] CanvasGroup resetPannel;

        [Header("Options - Access")]
        [SerializeField] OptionDropdown languageOption;
        [SerializeField] GameObject languageWarningText;
        [SerializeField] OptionSlider animationStrengthOption;
        [SerializeField] OptionToggle distanceToFlatOption;
        [SerializeField] OptionToggle consoleDashesStrengthOption;
        [SerializeField] OptionDropdown consoleBackgroundColor1Option;
        [SerializeField] OptionDropdown consoleBackgroundColor2Option;
        [SerializeField] OptionDropdown consoleTextColor1Option;
        [SerializeField] OptionDropdown consoleTextColor2Option;

        [Header("Options - Controls")]
        [SerializeField] OptionToggle invertYOption;
        [SerializeField] OptionSlider mouseSensitivityOption;
        [SerializeField] GameObject deleteConfirmationText;

        [Header("Options - Graphics")]
        [SerializeField] OptionToggle fullscreenOption;
        [SerializeField] OptionToggle vSyncOption;

        [Header("Options - Audio")]
        [SerializeField] OptionSlider masterVolumeOption;
        [SerializeField] OptionSlider ambianceVolumeOption;
        [SerializeField] OptionSlider sfxVolumeOption;
        [SerializeField] OptionSlider musicVolumeOption;
        [SerializeField] OptionToggle highFreqFilterOption;

        [Header("All Options")]
        [SerializeField] List<OptionBase> allOptions;

        public List<OptionBase> AllOptions => allOptions;

        public static OptionsMenu Instance;

        //events
        //  Access
        public static event Action<string, string> onLanguageChanged;
        public static event Action<string, float> onAnimationStrengthChanged;
        public static event Action<string, bool> onDistanceToFlatChanged;
        public static event Action<string, bool> onConsolDashesChanged;
        public static event Action<List<string>, List<string>> onConsolColorsChanged;
        //  Controls
        public static event Action<string, bool> onInvertYChanged;
        public static event Action<string, float> onLookSensitivityChanged;
        //  Graphics
        public static event Action<string, bool> onFullscreenChanged;
        public static event Action<string, bool> onVSyncChanged;
        //  Graphics
        public static event Action<string, float> onMasterVolumeChanged;
        public static event Action<string, float> onAmbianceVolumeChanged;
        public static event Action<string, float> onSFXVolumeChanged;
        public static event Action<string, float> onMusicVolumeChanged;
        public static event Action<string, bool> onHighFreqFilterChange;
        //  Reset
        public static event Action onResetButtonClicked;


        private void Awake()
        {
            Instance = this;

            TogglePannel(acessibilityPannel);

            //Buttons
            returnButton.onClick.AddListener(() => ToggleMenu(false));
            accessibilityButton.onClick.AddListener(() => TogglePannel(acessibilityPannel));
            controlsButton.onClick.AddListener(() => TogglePannel(controlsPannel));
            graphicsButton.onClick.AddListener(() => TogglePannel(graphicsPannel));
            audioButton.onClick.AddListener(() => TogglePannel(audioPannel));
            resetCategoryButton.onClick.AddListener(() => TogglePannel(resetPannel));
            resetButton.onClick.AddListener(() => OnClickResetButton());

            //Option base
            //  Access
            languageOption.onValueChanged += OnLanguageChange;
            animationStrengthOption.onValueChanged += OnAnimationStrengthChanged;
            distanceToFlatOption.onValueChanged += OnDistanceToFlatChanged;
            consoleDashesStrengthOption.onValueChanged += OnConsoleDashesChanged;
            consoleBackgroundColor1Option.onValueChanged += OnConsoleColorChange;
            consoleBackgroundColor2Option.onValueChanged += OnConsoleColorChange;
            consoleTextColor1Option.onValueChanged += OnConsoleColorChange;
            consoleTextColor2Option.onValueChanged += OnConsoleColorChange;

            //  Controls
            invertYOption.onValueChanged += OnInvertYChange;
            mouseSensitivityOption.onValueChanged += OnMouseSensitivityChange;

            //  Graphics
            fullscreenOption.onValueChanged += OnFullscreenChange;
            vSyncOption.onValueChanged += OnVSyncChange;

            //  Audio
            masterVolumeOption.onValueChanged += OnMasterVolumeChange;
            ambianceVolumeOption.onValueChanged += OnAmbianceVolumeChange;
            sfxVolumeOption.onValueChanged += OnSFXVolumeChange;
            musicVolumeOption.onValueChanged += OnMusicVolumeChange;
            highFreqFilterOption.onValueChanged += OnHighFreqFilterChange;
        }

        private void OnDestroy()
        {
            //Buttons
            returnButton.onClick.RemoveAllListeners();
            accessibilityButton.onClick.RemoveAllListeners();
            controlsButton.onClick.RemoveAllListeners();
            graphicsButton.onClick.RemoveAllListeners();
            audioButton.onClick.RemoveAllListeners();
            resetCategoryButton.onClick.RemoveAllListeners();
            resetButton.onClick.RemoveAllListeners();

            //Option base
            //  Access
            languageOption.onValueChanged -= OnLanguageChange;
            animationStrengthOption.onValueChanged -= OnAnimationStrengthChanged;
            distanceToFlatOption.onValueChanged -= OnDistanceToFlatChanged;
            consoleDashesStrengthOption.onValueChanged -= OnConsoleDashesChanged;
            consoleBackgroundColor1Option.onValueChanged -= OnConsoleColorChange;
            consoleBackgroundColor2Option.onValueChanged -= OnConsoleColorChange;
            consoleTextColor1Option.onValueChanged -= OnConsoleColorChange;
            consoleTextColor2Option.onValueChanged -= OnConsoleColorChange;

            //  Controls
            invertYOption.onValueChanged -= OnInvertYChange;
            mouseSensitivityOption.onValueChanged -= OnMouseSensitivityChange;

            //  Graphics
            fullscreenOption.onValueChanged -= OnFullscreenChange;
            vSyncOption.onValueChanged -= OnVSyncChange;

            //  Audio
            masterVolumeOption.onValueChanged -= OnMasterVolumeChange;
            ambianceVolumeOption.onValueChanged -= OnAmbianceVolumeChange;
            sfxVolumeOption.onValueChanged -= OnSFXVolumeChange;
            musicVolumeOption.onValueChanged -= OnMusicVolumeChange;
            highFreqFilterOption.onValueChanged -= OnHighFreqFilterChange;
        }

        public void InitializeAccessibilitySettings(
            string selectedLanguage,
            float animationStrength,
            bool useDistanceToFlat,
            bool useConsoleDashes,
            string colorBG1,
            string colorBG2,
            string colorTxt1,
            string colorTxt2
            )
        {
            languageOption.Init(selectedLanguage);
            animationStrengthOption.Init(animationStrength);
            distanceToFlatOption.Init(useDistanceToFlat);
            consoleDashesStrengthOption.Init(useConsoleDashes);
            consoleBackgroundColor1Option.Init(colorBG1);
            consoleBackgroundColor2Option.Init(colorBG2);
            consoleTextColor1Option.Init(colorTxt1);
            consoleTextColor2Option.Init(colorTxt2);
        }

        public void InitializeControlsSettings(
            bool invertY,
            float lookSensitivity)
        {
            rebindSaveLoad.LoadRebinds();

            invertYOption.Init(invertY);
            mouseSensitivityOption.Init(lookSensitivity);
        }

        public void InitializeGraphicsSettings(
            bool fullscreen,
            bool vSync)
        {
            fullscreenOption.Init(fullscreen);
            vSyncOption.Init(vSync);
        }

        public void InitializeAudioSettings(
            float masterVolume,
            float ambianceVolume,
            float sfxVolume,
            float musicVolume)
        {
            masterVolumeOption.Init(masterVolume);
            ambianceVolumeOption.Init(ambianceVolume);
            sfxVolumeOption.Init(sfxVolume);
            musicVolumeOption.Init(musicVolume);
        }

        public void ToggleMenu()
        {
            ToggleMenu(!optionsCanvasGroup.interactable);
        }

        public void ToggleMenu(bool isToggled, bool openedFromPauseMenu = true)
        {
            deleteConfirmationText.SetActive(false);
            languageWarningText.SetActive(openedFromPauseMenu);

            optionsCanvasGroup.alpha = isToggled ? 1 : 0;
            optionsCanvasGroup.interactable = isToggled;
            optionsCanvasGroup.blocksRaycasts = isToggled;
            StaticData.OpenWindowsAmount += isToggled ? 1 : -1;
            canvas.sortingOrder = isToggled ? StaticData.OpenWindowsAmount : -1;

            Action rebindAction = isToggled ? rebindSaveLoad.LoadRebinds : rebindSaveLoad.SaveRebinds;
            rebindAction?.Invoke();
        }

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

            ColorBlock colorBlock = new ColorBlock();
            colorBlock.normalColor = defaultButtonColor;
            colorBlock.highlightedColor = highlightedButtonColor;
            colorBlock.selectedColor = highlightedButtonColor;
            colorBlock.disabledColor = defaultButtonColor;
            colorBlock.pressedColor = clickedButtonColor;
            colorBlock.colorMultiplier = 1;

            accessibilityButton.colors = colorBlock;
            controlsButton.colors = colorBlock;
            graphicsButton.colors = colorBlock;
            audioButton.colors = colorBlock;
            resetCategoryButton.colors = colorBlock;


            ColorBlock clickedButtonColorBlock = new ColorBlock();
            clickedButtonColorBlock.normalColor = highlightedButtonColor;
            clickedButtonColorBlock.selectedColor = highlightedButtonColor;
            clickedButtonColorBlock.highlightedColor = highlightedButtonColor;
            clickedButtonColorBlock.disabledColor = defaultButtonColor;
            clickedButtonColorBlock.pressedColor = clickedButtonColor;
            clickedButtonColorBlock.colorMultiplier = 1;

            if (pannelToToggle == acessibilityPannel)
                accessibilityButton.colors = clickedButtonColorBlock;
            else if (pannelToToggle == controlsPannel)
                controlsButton.colors = clickedButtonColorBlock;
            else if (pannelToToggle == graphicsPannel)
                graphicsButton.colors = clickedButtonColorBlock;
            else if (pannelToToggle == audioPannel)
                audioButton.colors = clickedButtonColorBlock;
            else if (pannelToToggle == resetPannel)
                resetCategoryButton.colors = clickedButtonColorBlock;
        }

        #region OptionsCallback
        void OnLanguageChange()
        {
            onLanguageChanged?.Invoke(languageOption.ParameterName, languageOption.CurrentlySelectedString);
        }

        void OnAnimationStrengthChanged()
        {
            onAnimationStrengthChanged?.Invoke(animationStrengthOption.ParameterName, animationStrengthOption.Value);
        }

        void OnDistanceToFlatChanged()
        {
            onDistanceToFlatChanged?.Invoke(distanceToFlatOption.ParameterName, distanceToFlatOption.isToggled);
        }

        void OnConsoleDashesChanged()
        {
            onConsolDashesChanged?.Invoke(consoleDashesStrengthOption.ParameterName, consoleDashesStrengthOption.isToggled);
        }

        void OnConsoleColorChange()
        {
            List<string> keys = new List<string>(new string[] {
            consoleBackgroundColor1Option.ParameterName,
            consoleBackgroundColor2Option.ParameterName,
            consoleTextColor1Option.ParameterName,
            consoleTextColor2Option.ParameterName});

            List<string> colors = new List<string>(new string[] {
            consoleBackgroundColor1Option.CurrentlySelectedString,
            consoleBackgroundColor2Option.CurrentlySelectedString,
            consoleTextColor1Option.CurrentlySelectedString,
            consoleTextColor2Option.CurrentlySelectedString});

            onConsolColorsChanged?.Invoke(keys, colors);
        }

        void OnInvertYChange()
        {
            onInvertYChanged?.Invoke(invertYOption.ParameterName, invertYOption.isToggled);
        }

        private void OnClickResetButton()
        {
            onResetButtonClicked?.Invoke();
            deleteConfirmationText.SetActive(true);
        }

        void OnMouseSensitivityChange()
        {
            onLookSensitivityChanged?.Invoke(mouseSensitivityOption.ParameterName, mouseSensitivityOption.Value);
        }

        void OnFullscreenChange()
        {
            onFullscreenChanged?.Invoke(fullscreenOption.ParameterName, fullscreenOption.isToggled);
        }

        void OnVSyncChange()
        {
            onVSyncChanged?.Invoke(vSyncOption.ParameterName, vSyncOption.isToggled);
        }

        private void OnMasterVolumeChange()
        {
            onMasterVolumeChanged?.Invoke(masterVolumeOption.ParameterName, masterVolumeOption.Value);
        }

        private void OnAmbianceVolumeChange()
        {
            onAmbianceVolumeChanged?.Invoke(ambianceVolumeOption.ParameterName, ambianceVolumeOption.Value);
        }

        private void OnSFXVolumeChange()
        {
            onSFXVolumeChanged?.Invoke(sfxVolumeOption.ParameterName, sfxVolumeOption.Value);
        }

        private void OnMusicVolumeChange()
        {
            onMusicVolumeChanged?.Invoke(musicVolumeOption.ParameterName, musicVolumeOption.Value);
        }

        private void OnHighFreqFilterChange()
        {
            onHighFreqFilterChange?.Invoke(highFreqFilterOption.ParameterName, highFreqFilterOption.isToggled);
        }
        #endregion OptionsCallback
    }
}