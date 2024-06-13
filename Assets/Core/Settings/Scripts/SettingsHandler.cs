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
        //TODO : Load and apply settings

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

    private void OnLanguageChanged(string key, string newLanguage)
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

    private void OnAnimStrengthChanged(string key, float newValue)
    {
        Debug.LogWarning("TODO");
    }

    private void OnDistanceToFlatChanged(string key, bool newValue)
    {
        Debug.LogWarning("TODO");
    }

    private void OnConsoleDashesChanged(string key, bool newValue)
    {
        Debug.LogWarning("TODO");
    }

    private void OnConsoleColorsChanged(List<string> keys, List<string> newValues)
    {
        Debug.LogWarning("TODO");
    }

    private void OnInvertYChanged(string key, bool newValue)
    {
        Debug.LogWarning("TODO");
    }

    private void OnLookSensitivityChanged(string key, float newValue)
    {
        Debug.LogWarning("TODO");
    }

    private void OnFullScreenChanged(string key, bool newValue)
    {
        Debug.LogWarning("TODO");
    }

    private void OnVSyncChanged(string key, bool newValue)
    {
        Debug.LogWarning("TODO");
    }

    private void OnMasterVolumeChanged(string key, float newValue)
    {
        Debug.LogWarning("TODO");
    }

    private void OnAmbianceVolumeChanged(string key, float newValue)
    {
        Debug.LogWarning("TODO");
    }

    private void OnSFXVolumeChanged(string key, float newValue)
    {
        Debug.LogWarning("TODO");
    }

    private void OnMusicVolumeChanged(string key, float newValue)
    {
        Debug.LogWarning("TODO");
    }
}
