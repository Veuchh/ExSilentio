using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.SceneManagement;

namespace LW.Level
{
    public class StartupHandler : MonoBehaviour
    {
        [Scene, SerializeField] string mainMenuScene;

        void Start()
        {
            SceneManager.LoadScene(mainMenuScene);
        }
    }
}