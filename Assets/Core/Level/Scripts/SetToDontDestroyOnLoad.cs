using UnityEngine;

namespace LW.Level
{
    public class SetToDontDestroyOnLoad : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}