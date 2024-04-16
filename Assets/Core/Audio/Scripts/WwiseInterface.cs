using System;
using UnityEngine;

namespace LW.Audio
{
    public class WwiseInterface : MonoBehaviour
    {
        public static WwiseInterface Instance;
        string currentLevelBank;
        GameObject playerCamera;

        private void Awake()
        {
            Instance = this;
        }

        public void UpdatePlayerCamera(GameObject newCamera)
        {
            playerCamera = newCamera;
        }

        public void SetSwitch(AK.Wwise.Switch mySwitch, GameObject origin = null)
        {
            if (origin == null)
            {
                if (playerCamera != null)
                {
                    origin = playerCamera;
                }
                else
                {
                    Debug.LogError("No camera was set");
                    return;
                }
            }
            mySwitch.SetValue(origin);
        }

        public void RemovePlayerCameraIfCameraIsThis(GameObject camToRemove)
        {
            if (camToRemove == playerCamera)
                playerCamera = null;
        }

        public void PlayEvent(AK.Wwise.Event eventToPlay, GameObject origin = null)
        {
            if (origin == null)
            {
                if (playerCamera != null)
                {
                    origin = playerCamera;
                }
                else
                {
                    Debug.LogError("No camera was set");
                    return;
                }
            }

            AkSoundEngine.PostEvent(eventToPlay.Id, origin);
        }

        public void ChangeCurrentLevelBank(string newBankName)
        {
            if (!string.IsNullOrEmpty(currentLevelBank))
                AkBankManager.UnloadBank(currentLevelBank);

            currentLevelBank = newBankName;
            AkBankManager.LoadBankAsync(currentLevelBank);
        }
    }
}