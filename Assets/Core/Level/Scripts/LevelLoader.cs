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
        [SerializeField, Scene] List<string> levels;
        [SerializeField] AK.Wwise.Event loadLevelAudio;

        public static LevelLoader Instance;
        public List<string> Levels => levels;

        void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(this);
            Debug.LogWarning("TODO : make it load mycelium instead of ocean city");
            MainMenu.onMainMenuPressPlay += () => ChangeLevel("Mycelium", "Mycelium");
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
            var asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);
            LoadingScreen.Instance.ToggleLoadingScreen(true, translatedSceneName);

            while (asyncOperation.progress < 1)
            {
                yield return null;
                LoadingScreen.Instance.UpdateProgress(asyncOperation.progress);
            }

            yield return null;
            yield return new WaitForSeconds(.2f);

            LoadingScreen.Instance.ToggleLoadingScreen(false, translatedSceneName);
        }
    }
}