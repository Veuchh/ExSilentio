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

    public static void ClearSavedWords(List<string> keysToKeep)
    {
        //Add settings to a buffer
        Debug.LogWarning("Clear all words is not fully implemented and will also delete all your settings");

        //Clear all playerPrefs
        PlayerPrefs.DeleteAll();

        //Add buffer back to playerPrefs
    }
    }
