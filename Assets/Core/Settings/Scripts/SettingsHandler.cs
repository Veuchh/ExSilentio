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
        OptionsMenu.onLanguageChanged += OnLanguageChanged;
    }

    private void OnDestroy()
    {
        OptionsMenu.onLanguageChanged -= OnLanguageChanged;
    }

    private void OnLanguageChanged(string newLanguage)
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
}
