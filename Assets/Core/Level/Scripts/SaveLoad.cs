using System;
using System.Collections.Generic;
using UnityEngine;

public static class SaveLoad
{
    public const string SCENE_WITH_ALL_CORE_REVEALED = "scenesWithAllCoreRevealed";

    public static bool IsValueSaved(string key) => PlayerPrefs.HasKey(key);

    public static void SaveStringToPlayerPrefs(string key, string word)
    {
        PlayerPrefs.SetString(key, word);
    }
    public static void SaveFloatToPlayerPrefs(string key, float newValue)
    {
        PlayerPrefs.SetFloat(key, newValue);
    }

    public static void SaveIntToPlayerPrefs(string key, int newValue)
    {
        PlayerPrefs.SetInt(key, newValue);
    }

    public static int GetIntFromPlayerPrefs(string key)
    {
        return PlayerPrefs.GetInt(key);
    }

    public static string GetStringFromPlayerPrefs(string key)
    {
        return PlayerPrefs.GetString(key);
    }

    public static float GetFloatFromPlayerPrefs(string key)
    {
        return PlayerPrefs.GetFloat(key);
    }

    public static void ClearSavedWords(Dictionary<string, int> intOptions,
        Dictionary<string, float> floatOptions,
        Dictionary<string, string> stringOptions,
        string bindingsSettingsKey)
    {
        string bindingsSettings = PlayerPrefs.GetString(bindingsSettingsKey);

        //Clear all playerPrefs
        PlayerPrefs.DeleteAll();

        PlayerPrefs.SetString(bindingsSettingsKey, bindingsSettings);

        //Add buffer back to playerPrefs
        foreach (var item in intOptions.Keys)
            SaveIntToPlayerPrefs(item, intOptions[item]);

        foreach (var item in floatOptions.Keys)
            SaveFloatToPlayerPrefs(item, floatOptions[item]);

        foreach (var item in stringOptions.Keys)
            SaveStringToPlayerPrefs(item, stringOptions[item]);
    }

    public static void ClearAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}
