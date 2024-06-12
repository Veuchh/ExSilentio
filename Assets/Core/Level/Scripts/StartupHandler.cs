using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LW.Level
{
    public class StartupHandler : MonoBehaviour
    {
        [Scene, SerializeField] string mainMenuScene;

        void Start()
        {
            SceneManager.LoadScene(mainMenuScene);

            //TODO Remove stub
            SaveLoad.ClearSavedWords();
        }
    }
}