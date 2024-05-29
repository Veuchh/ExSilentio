using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class ForceLanguage : MonoBehaviour
{
    [SerializeField]Locale locale;
    private void Awake()
    {
        LocalizationSettings.SelectedLocale = locale;
    }
}
