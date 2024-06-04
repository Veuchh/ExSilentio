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
}
