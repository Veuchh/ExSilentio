using LW.Audio;
using LW.Data;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    [SerializeField, Scene] List<string> levels;

    public static LevelLoader Instance;
    public List<string> Levels => levels;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }

    public FunctionResult ChangeLevel(string args, string baseInput)
    {
        if (SceneManager.GetActiveScene().name.ToLower() == args.ToLower())
            return FunctionResult.Interrupted;

        // Get build scenes
        var sceneNumber = SceneManager.sceneCountInBuildSettings;

        for (int i = 0; i < sceneNumber; i++)
        {
            string sceneName = Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i).ToLower());

            if (sceneName == args.ToLower())
            {
                WwiseInterface.Instance.StopAll();
                StartCoroutine(LoadScene(i, baseInput));
                return FunctionResult.Success;
            }
        }

        Debug.Log("No scene with that name exists");
        return FunctionResult.Failed;
    }

    IEnumerator LoadScene(int sceneIndex, string translatedSceneName)
    {
        var asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);
        LoadingScreen.Instance.ToggleLoadingScreen(true, translatedSceneName);

        while (asyncOperation.progress < 1)
        {
            yield return null;
            LoadingScreen.Instance.UpdateProgress(asyncOperation.progress);
        }

        yield return null;

        LoadingScreen.Instance.ToggleLoadingScreen(false, translatedSceneName);
    }
}
