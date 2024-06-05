using UnityEngine;

public static class SaveLoad
{
    public static void SaveWordToPlayerPrefs(string bundleKey, string word)
    {
        PlayerPrefs.SetString(bundleKey, word);
    }

    public static string GetWordFromPlayerPrefs(string bundleKey)
    {
        return PlayerPrefs.GetString(bundleKey);
    }

    public static void ClearSavedWords()
    {
        //Add settings to a buffer
        Debug.LogWarning("Clear all words is not fully implemented and will also delete all your settings");

        //Clear all playerPrefs
        PlayerPrefs.DeleteAll();

        //Add buffer back to playerPrefs
    }
}
