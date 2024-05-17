using System;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;
using static UnityEngine.UI.Image;

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

        public void RemovePlayerCameraIfCameraIsThis(GameObject camToRemove)
        {
            if (camToRemove == playerCamera)
                playerCamera = null;
        }

        public void ChangeSwitch(AK.Wwise.Switch switchToSet, GameObject origin = null)
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

            AkSoundEngine.SetSwitch(switchToSet.GroupId, switchToSet.Id, origin);
        }

        public uint PlayEvent(AK.Wwise.Event eventToPlay, GameObject origin = null)
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
                    return 0;
                }
            }

            return AkSoundEngine.PostEvent(eventToPlay.Id, origin);
        }

        public void StopEvent(uint idToStop, GameObject origin = null)
        {
            AkSoundEngine.StopPlayingID(idToStop);
        }

        public void ChangeCurrentLevelBank(string newBankName)
        {
            if (!string.IsNullOrEmpty(currentLevelBank))
                AkBankManager.UnloadBank(currentLevelBank);

            currentLevelBank = newBankName;
            AkBankManager.LoadBankAsync(currentLevelBank);
        }

        public void UpdateRTPC(string rtpcName, float rtpcValue, GameObject origin = null)
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

            AkSoundEngine.SetRTPCValue(rtpcName, rtpcValue, origin);
        }
    }
}