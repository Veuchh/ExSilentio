using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
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
        SceneManager.LoadScene(args);
    }
}
