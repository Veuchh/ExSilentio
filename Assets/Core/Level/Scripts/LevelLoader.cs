using LW.Audio;
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

    public void ChangeLevel(string args)
    {
        // Get build scenes
        var sceneNumber = SceneManager.sceneCountInBuildSettings;

        for (int i = 0; i < sceneNumber; i++)
        {
            string sceneName = Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i).ToLower());

            if (sceneName == args.ToLower())
            {
                WwiseInterface.Instance.StopAll();
                SceneManager.LoadSceneAsync(i);
                return;
            }
        }

        Debug.Log("No scene with that name exists");
    }
}
