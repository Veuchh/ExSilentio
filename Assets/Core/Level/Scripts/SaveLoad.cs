using System;
using System.Collections.Generic;
using UnityEngine;

public static class SaveLoad
{
    public const string SCENE_WITH_ALL_CORE_REVEALED = "scenesWithAllCoreRevealed";

    public static bool IsValueSaved(string key) => PlayerPrefs.HasKey(key);

    public static void SaveStringToPlayerPrefs(string bundleKey, string word)
    {
        PlayerPrefs.SetString(bundleKey, word);
    }

    public static string GetWordFromPlayerPrefs(string bundleKey)
    {
        return PlayerPrefs.GetString(bundleKey);
    }

    public static void SaveFloatToPlayerPrefs(string key, float newValue)
    {
        PlayerPrefs.SetFloat(key, newValue);
    }

    public static void ClearSavedWords(List<string> keysToKeep)
    {
        //Add settings to a buffer
        Debug.LogWarning("Clear all words is not fully implemented and will also delete all your settings");

        //Clear all playerPrefs
        PlayerPrefs.DeleteAll();

        //Add buffer back to playerPrefs
    }

    public static void SaveIntToPlayerPrefs(string bundleKey, int newValue)
    {
        PlayerPrefs.SetInt(bundleKey, newValue);
    }

    public static int GetIntFromPlayerPrefs(string bundleKey)
    {
        return PlayerPrefs.GetInt(bundleKey);
    }
}
