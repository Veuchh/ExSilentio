using AK.Wwise;
using LW.Audio;
using LW.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

/// <summary>
/// This script is responsible for applying and saving the settings.
/// It is mainly called by the options menu.
/// </summary>
public class SettingsHandler : MonoBehaviour
{
    [Header("Language")]
    [SerializeField] Locale english;
    [SerializeField] Locale french;

    [Header("Audio")]
    [SerializeField] RTPC masterRTPC;
    [SerializeField] RTPC ambianceRTPC;
    [SerializeField] RTPC sfxRTPC;
    [SerializeField] RTPC musicRTPC;

    [Header("Colors")]
    [SerializeField] private Color blackColor;
    [SerializeField] private Color greyColor;
    [SerializeField] private Color darkGreyColor;
    [SerializeField] private Color whiteColor;
    [SerializeField] private Color redColor;
    [SerializeField] private Color darkRedColor;
    [SerializeField] private Color blueColor;
    [SerializeField] private Color darkBlueColor;
    [SerializeField] private Color greenColor;
    [SerializeField] private Color darkGreenColor;
    [SerializeField] private Color yellowColor;
    [SerializeField] private Color darkYellowColor;
    [SerializeField] private Color magentaColor;
    [SerializeField] private Color darkMagentaColor;
    [SerializeField] private Color cyanColor;
    [SerializeField] private Color darkCyanColor;

    void Start()
    {
        LoadSettings();

        //  Access
        OptionsMenu.onLanguageChanged += OnLanguageChanged;
        OptionsMenu.onAnimationStrengthChanged += OnAnimStrengthChanged;
        OptionsMenu.onDistanceToFlatChanged += OnDistanceToFlatChanged;
        OptionsMenu.onConsolDashesChanged += OnConsoleDashesChanged;
        OptionsMenu.onConsolColorsChanged += OnConsoleColorsChanged;

        //  Controls
        OptionsMenu.onLookSensitivityChanged += OnLookSensitivityChanged;
        OptionsMenu.onInvertYChanged += OnInvertYChanged;

        //  Graphics
        OptionsMenu.onFullscreenChanged += OnFullScreenChanged;
        OptionsMenu.onVSyncChanged += OnVSyncChanged;

        //  Audio
        OptionsMenu.onMasterVolumeChanged += OnMasterVolumeChanged;
        OptionsMenu.onAmbianceVolumeChanged += OnAmbianceVolumeChanged;
        OptionsMenu.onSFXVolumeChanged += OnSFXVolumeChanged;
        OptionsMenu.onMusicVolumeChanged += OnMusicVolumeChanged;
    }

    private void OnDestroy()
    {
        //  Access
        OptionsMenu.onLanguageChanged -= OnLanguageChanged;
        OptionsMenu.onAnimationStrengthChanged -= OnAnimStrengthChanged;
        OptionsMenu.onDistanceToFlatChanged -= OnDistanceToFlatChanged;
        OptionsMenu.onConsolDashesChanged -= OnConsoleDashesChanged;
        OptionsMenu.onConsolColorsChanged -= OnConsoleColorsChanged;

        //  Controls
        OptionsMenu.onLookSensitivityChanged -= OnLookSensitivityChanged;
        OptionsMenu.onInvertYChanged -= OnInvertYChanged;

        //  Graphics
        OptionsMenu.onFullscreenChanged -= OnFullScreenChanged;
        OptionsMenu.onVSyncChanged -= OnVSyncChanged;

        //  Audio
        OptionsMenu.onMasterVolumeChanged -= OnMasterVolumeChanged;
        OptionsMenu.onAmbianceVolumeChanged -= OnAmbianceVolumeChanged;
        OptionsMenu.onSFXVolumeChanged -= OnSFXVolumeChanged;
        OptionsMenu.onMusicVolumeChanged -= OnMusicVolumeChanged;
    }

    void LoadSettings()
    {
        Dictionary<string, int> intOptions = new Dictionary<string, int>();
        Dictionary<string, float> floatOptions = new Dictionary<string, float>();
        Dictionary<string, string> stringOptions = new Dictionary<string, string>();

        foreach (OptionBase option in OptionsMenu.Instance.AllOptions)
        {
            if (SaveLoad.IsValueSaved(option.ParameterName))
            {
                switch (option)
                {
                    case OptionSlider slider:
                        floatOptions.Add(option.ParameterName, SaveLoad.GetFloatFromPlayerPrefs(option.ParameterName));
                        break;
                    case OptionDropdown dropdown:
                        stringOptions.Add(option.ParameterName, SaveLoad.GetStringFromPlayerPrefs(option.ParameterName));
                        break;
                    case OptionToggle toggle:
                        intOptions.Add(option.ParameterName, SaveLoad.GetIntFromPlayerPrefs(option.ParameterName));
                        break;
                }
            }
        }

        //Set proper values to optins menu
        //  Access
        OptionsMenu.Instance.InitializeAccessibilitySettings(
            selectedLanguage: (stringOptions.Keys.Contains("language") ? stringOptions["language"] : "English"),
            animationStrength: (floatOptions.Keys.Contains("animationStrength") ? floatOptions["animationStrength"] : 1),
            useDistanceToFlat: (intOptions.Keys.Contains("distanceToFlat") ? intOptions["distanceToFlat"] == 1 : false),
            useConsoleDashes: (intOptions.Keys.Contains("consoleDashes") ? intOptions["consoleDashes"] == 1 : false),
            colorBG1: (stringOptions.Keys.Contains("colorBG1") ? stringOptions["colorBG1"] : "Black"),
            colorBG2: (stringOptions.Keys.Contains("colorBG2") ? stringOptions["colorBG2"] : "DarkGrey"),
            colorTxt1: (stringOptions.Keys.Contains("colorTxt1") ? stringOptions["colorTxt1"] : "Green"),
            colorTxt2: (stringOptions.Keys.Contains("colorTxt2") ? stringOptions["colorTxt2"] : "Green"));

        //  Controls
        OptionsMenu.Instance.InitializeControlsSettings(
            invertY: (intOptions.Keys.Contains("invertY") ? intOptions["invertY"] == 1 : false),
            lookSensitivity: (floatOptions.Keys.Contains("lookSensitivity") ? floatOptions["lookSensitivity"] : 1));

        //  Graphics
        OptionsMenu.Instance.InitializeGraphicsSettings(
            fullscreen: (intOptions.Keys.Contains("fullscreen") ? intOptions["fullscreen"] == 1 : true),
            vSync: (intOptions.Keys.Contains("vsync") ? intOptions["vsync"] == 1 : false));

        //  Audio
        OptionsMenu.Instance.InitializeAudioSettings(
            masterVolume: floatOptions.Keys.Contains("masterVolume") ? floatOptions["masterVolume"] : 50,
            ambianceVolume: floatOptions.Keys.Contains("ambianceVolume") ? floatOptions["ambianceVolume"] : 50,
            sfxVolume: floatOptions.Keys.Contains("sfxVolume") ? floatOptions["sfxVolume"] : 50,
            musicVolume: floatOptions.Keys.Contains("musicVolume") ? floatOptions["musicVolume"] : 50);

        //Apply settings
        //  Access
        ChangeLanguage(stringOptions.Keys.Contains("language") ? stringOptions["language"] : "English");
        List<string> consoleColors = new List<string> {
            (stringOptions.Keys.Contains("colorBG1") ? stringOptions["colorBG1"] : "Black"),
            (stringOptions.Keys.Contains("colorBG2") ? stringOptions["colorBG2"] : "DarkGrey"),
             (stringOptions.Keys.Contains("colorTxt1") ? stringOptions["colorTxt1"] : "Green"),
             (stringOptions.Keys.Contains("colorTxt2") ? stringOptions["colorTxt2"] : "Green")
        };
        ChangeConsoleColors(consoleColors);
        ChangeConsoleDashes(intOptions.Keys.Contains("consoleDashes") ? intOptions["consoleDashes"] == 1 : false);

        //  Graphics
        ChangeFullscreen(intOptions.Keys.Contains("fullscreen") ? intOptions["fullscreen"] == 1 : true);
        ChangeVSync(intOptions.Keys.Contains("vsync") ? intOptions["vsync"] == 1 : false);

        //  Audio
        ChangeMasterVolume(floatOptions.Keys.Contains("masterVolume") ? floatOptions["masterVolume"] : 50);
        ChangeAmbainceVolume(floatOptions.Keys.Contains("ambianceVolume") ? floatOptions["ambianceVolume"] : 50);
        ChangeSFXVolume(floatOptions.Keys.Contains("sfxVolume") ? floatOptions["sfxVolume"] : 50);
        ChangeMusicVolume(floatOptions.Keys.Contains("musicVolume") ? floatOptions["musicVolume"] : 50);
    }

    private void ChangeLanguage(string newLanguage)
    {
        switch (newLanguage.ToLower())
        {
            case "english":
                LocalizationSettings.SelectedLocale = english;
                break;
            case "français":
                LocalizationSettings.SelectedLocale = french;
                break;
        }
    }

    private void ChangeFullscreen(bool isToggled)
    {
        Screen.fullScreen = isToggled;
    }

    private void ChangeVSync(bool isToggled)
    {
        QualitySettings.vSyncCount = isToggled ? 1 : 0;
    }

    private void ChangeConsoleDashes(bool isToggled)
    {
        ConsoleUI.Instance.UpdateConsoleDashes(isToggled);
    }

    private void ChangeConsoleColors(List<string> colors)
    {
        List<Color> newColors = new List<Color>();

        for (int i = 0; i < 4; i++)
        {
            Color color = Color.black;

            switch (colors[i])
            {
                case "Black":
                    newColors.Add(blackColor);
                    break;
                case "Grey":
                    newColors.Add(greyColor);
                    break;
                case "DarkGrey":
                    newColors.Add(darkGreyColor);
                    break;
                case "White":
                    newColors.Add(whiteColor);
                    break;
                case "Red":
                    newColors.Add(redColor);
                    break;
                case "DarkRed":
                    newColors.Add(darkRedColor);
                    break;
                case "Blue":
                    newColors.Add(blueColor);
                    break;
                case "DarkBlue":
                    newColors.Add(darkBlueColor);
                    break;
                case "Green":
                    newColors.Add(greenColor);
                    break;
                case "DarkGreen":
                    newColors.Add(darkGreenColor);
                    break;
                case "Yellow":
                    newColors.Add(yellowColor);
                    break;
                case "DarkYellow":
                    newColors.Add(darkYellowColor);
                    break;
                case "Magenta":
                    newColors.Add(magentaColor);
                    break;
                case "DarkMagenta":
                    newColors.Add(darkMagentaColor);
                    break;
                case "Cyan":
                    newColors.Add(cyanColor);
                    break;
                case "DarkCyan":
                    newColors.Add(darkCyanColor);
                    break;
                default:
                    Debug.LogError($"Wrong color specified : {colors[i]}");
                    newColors.Add(Color.black);
                    break;
            }
        }

        ConsoleUI.Instance.UpdateConsoleColor(newColors);
    }

    void ChangeMasterVolume(float newValue)
    {
        WwiseInterface.Instance.SetGlobalRTPCValue(masterRTPC, newValue);
    }

    void ChangeAmbainceVolume(float newValue)
    {
        WwiseInterface.Instance.SetGlobalRTPCValue(ambianceRTPC, newValue);
    }

    void ChangeSFXVolume(float newValue)
    {
        WwiseInterface.Instance.SetGlobalRTPCValue(sfxRTPC, newValue);
    }

    void ChangeMusicVolume(float newValue)
    {
        WwiseInterface.Instance.SetGlobalRTPCValue(musicRTPC, newValue);
    }

    private void OnLanguageChanged(string key, string newLanguage)
    {
        SaveLoad.SaveStringToPlayerPrefs(key, newLanguage);

        ChangeLanguage(newLanguage);
    }


    private void OnAnimStrengthChanged(string key, float newValue)
    {
        SaveLoad.SaveFloatToPlayerPrefs(key, newValue);

        Debug.LogWarning("TODO : CHANGE ANIM STRENGTH");
    }

    private void OnDistanceToFlatChanged(string key, bool newValue)
    {
        SaveLoad.SaveIntToPlayerPrefs(key, newValue == true ? 1 : 0);

        Debug.LogWarning("TODO : DistanceToFlat");
    }

    private void OnConsoleDashesChanged(string key, bool newValue)
    {
        SaveLoad.SaveIntToPlayerPrefs(key, newValue == true ? 1 : 0);

        ChangeConsoleDashes(newValue);
    }

    private void OnConsoleColorsChanged(List<string> keys, List<string> newValues)
    {
        for (int i = 0; i < keys.Count; i++)
        {
            SaveLoad.SaveStringToPlayerPrefs(keys[i], newValues[i]);
        }

        ChangeConsoleColors(newValues);
    }

    private void OnInvertYChanged(string key, bool newValue)
    {
        SaveLoad.SaveIntToPlayerPrefs(key, newValue == true ? 1 : 0);

        Debug.LogWarning("TODO : InvertY");
    }

    private void OnLookSensitivityChanged(string key, float newValue)
    {
        SaveLoad.SaveFloatToPlayerPrefs(key, newValue);

        Debug.LogWarning("TODO : LookSensitivity");
    }

    private void OnFullScreenChanged(string key, bool newValue)
    {
        SaveLoad.SaveIntToPlayerPrefs(key, newValue == true ? 1 : 0);

        ChangeFullscreen(newValue);
    }

    private void OnVSyncChanged(string key, bool newValue)
    {
        SaveLoad.SaveIntToPlayerPrefs(key, newValue == true ? 1 : 0);

        ChangeVSync(newValue);
    }

    private void OnMasterVolumeChanged(string key, float newValue)
    {
        SaveLoad.SaveFloatToPlayerPrefs(key, newValue);

        ChangeMasterVolume(newValue);

        Debug.LogWarning("TODO : PLAY AUDIO");
    }

    private void OnAmbianceVolumeChanged(string key, float newValue)
    {
        SaveLoad.SaveFloatToPlayerPrefs(key, newValue);

        ChangeAmbainceVolume(newValue);

        Debug.LogWarning("TODO : PLAY AUDIO");
    }

    private void OnSFXVolumeChanged(string key, float newValue)
    {
        SaveLoad.SaveFloatToPlayerPrefs(key, newValue);

        ChangeSFXVolume(newValue);

        Debug.LogWarning("TODO : PLAY AUDIO");
    }

    private void OnMusicVolumeChanged(string key, float newValue)
    {
        SaveLoad.SaveFloatToPlayerPrefs(key, newValue);

        ChangeMusicVolume(newValue);

        Debug.LogWarning("TODO : PLAY AUDIO");
    }
}
