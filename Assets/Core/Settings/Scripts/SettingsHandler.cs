using LW.UI;
using NaughtyAttributes;
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

        //Acess
        OptionsMenu.Instance.InitializeAccessibilitySettings(
            selectedLanguage: (stringOptions.Keys.Contains("language") ? stringOptions["language"] : "English"),
            animationStrength: (floatOptions.Keys.Contains("animationStrength") ? floatOptions["animationStrength"] : 1),
            useDistanceToFlat: (intOptions.Keys.Contains("distanceToFlat") ? intOptions["distanceToFlat"] == 1 : false),
            useConsoleDashes: (intOptions.Keys.Contains("consoleDashes") ? intOptions["consoleDashes"] == 1 : false),
            colorBG1: (stringOptions.Keys.Contains("colorBG1") ? stringOptions["colorBG1"] : "Black"),
            colorBG2: (stringOptions.Keys.Contains("colorBG2") ? stringOptions["colorBG2"] : "Grey"),
            colorTxt1: (stringOptions.Keys.Contains("colorTxt1") ? stringOptions["colorTxt1"] : "Green"),
            colorTxt2: (stringOptions.Keys.Contains("colorTxt2") ? stringOptions["colorTxt2"] : "Green"));

        //Controls
        OptionsMenu.Instance.InitializeControlsSettings(
            invertY: (intOptions.Keys.Contains("invertY") ? intOptions["invertY"] == 1 : false),
            lookSensitivity: (floatOptions.Keys.Contains("lookSensitivity") ? floatOptions["lookSensitivity"] : 1));

        //Graphics
        OptionsMenu.Instance.InitializeGraphicsSettings(
            fullscreen: (intOptions.Keys.Contains("fullscreen") ? intOptions["fullscreen"] == 1 : true),
            vSync: (intOptions.Keys.Contains("vsync") ? intOptions["vsync"] == 1 : false));

        //Audio
        OptionsMenu.Instance.InitializeAudioSettings(
            masterVolume: floatOptions.Keys.Contains("masterVolume") ? floatOptions["masterVolume"] : 50,
            ambianceVolume: floatOptions.Keys.Contains("ambianceVolume") ? floatOptions["ambianceVolume"] : 50,
            sfxVolume: floatOptions.Keys.Contains("sfxVolume") ? floatOptions["sfxVolume"] : 50,
            musicVolume: floatOptions.Keys.Contains("musicVolume") ? floatOptions["musicVolume"] : 50);

        //Apply settings
        ChangeLanguage(stringOptions.Keys.Contains("language") ? stringOptions["language"] : "English");
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

        Debug.LogWarning("TODO : ConsoleDashes");
    }

    private void OnConsoleColorsChanged(List<string> keys, List<string> newValues)
    {
        for (int i = 0; i < keys.Count; i++)
        {
            SaveLoad.SaveStringToPlayerPrefs(keys[i], newValues[i]);
        }

        Debug.LogWarning("TODO : Console Colors");
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

        Debug.LogWarning("TODO : Fullscreen");
    }

    private void OnVSyncChanged(string key, bool newValue)
    {
        SaveLoad.SaveIntToPlayerPrefs(key, newValue == true ? 1 : 0);

        Debug.LogWarning("TODO : VSync");
    }

    private void OnMasterVolumeChanged(string key, float newValue)
    {
        SaveLoad.SaveFloatToPlayerPrefs(key, newValue);

        Debug.LogWarning("TODO : SLIDERS AUDIO");
    }

    private void OnAmbianceVolumeChanged(string key, float newValue)
    {
        SaveLoad.SaveFloatToPlayerPrefs(key, newValue);

        Debug.LogWarning("TODO : SLIDERS AUDIO\"");
    }

    private void OnSFXVolumeChanged(string key, float newValue)
    {
        SaveLoad.SaveFloatToPlayerPrefs(key, newValue);

        Debug.LogWarning("TODO : SLIDERS AUDIO\"");
    }

    private void OnMusicVolumeChanged(string key, float newValue)
    {
        SaveLoad.SaveFloatToPlayerPrefs(key, newValue);

        Debug.LogWarning("TODO : SLIDERS AUDIO\"");
    }
}
