using LW.Audio;
using LW.Data;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LW.Level
{
    public class LevelLoader : MonoBehaviour
    {
        const string DISSOLVE_ID = "_EffectAmount";
        [SerializeField, Scene] List<string> levels;
        [SerializeField] AK.Wwise.Event loadLevelAudio;
        [SerializeField] Material screenMaterial;
        [SerializeField] float fadeDuration = .5f;

        public static LevelLoader Instance;
        public List<string> Levels => levels;

        void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(this);
            Debug.LogWarning("TODO : make it load mycelium instead of ocean city");
            MainMenu.onMainMenuPressPlay += () => ChangeLevel("Mycelium", "Mycelium");
            screenMaterial.SetFloat(DISSOLVE_ID, 1);
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
                    WwiseInterface.Instance.PlayEvent(loadLevelAudio, gameObject);
                    StartCoroutine(LoadScene(i, baseInput));
                    return FunctionResult.Success;
                }
            }

            Debug.Log("No scene with that name exists");
            return FunctionResult.Failed;
        }

        IEnumerator LoadScene(int sceneIndex, string translatedSceneName)
        {
            float startFadeTime = Time.time;
            float endFadeTime = Time.time + fadeDuration;

            while (Time.time < endFadeTime)
            {
                screenMaterial.SetFloat(DISSOLVE_ID, Mathf.InverseLerp(endFadeTime, startFadeTime, Time.time));
                yield return null;
            }

            screenMaterial.SetFloat(DISSOLVE_ID, 0);

            var asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);
            LoadingScreen.Instance.ToggleLoadingScreen(true, translatedSceneName);

            while (asyncOperation.progress < 1)
            {
                yield return null;
                LoadingScreen.Instance.UpdateProgress(asyncOperation.progress);
            }

            startFadeTime = Time.time;
            endFadeTime = Time.time + fadeDuration;

            while (Time.time < endFadeTime)
            {
                screenMaterial.SetFloat(DISSOLVE_ID, Mathf.InverseLerp(startFadeTime, endFadeTime, Time.time));
                yield return null;
            }

            screenMaterial.SetFloat(DISSOLVE_ID, 1);

            LoadingScreen.Instance.ToggleLoadingScreen(false, translatedSceneName);
        }
    }
}