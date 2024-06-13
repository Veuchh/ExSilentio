using LW.UI;
using System;
using System.Collections.Generic;
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

        OptionsMenu.Instance.InitializeAccessibilitySettings();
        OptionsMenu.Instance.InitializeControlsSettings();
        OptionsMenu.Instance.InitializeGraphicsSettings();
        OptionsMenu.Instance.InitializeAudioSettings();

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
        foreach (OptionBase option in OptionsMenu.Instance.AllOptions)
        {

        }
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
