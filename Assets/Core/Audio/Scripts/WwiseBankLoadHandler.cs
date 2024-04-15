using UnityEngine;

namespace LW.Audio
{
    public class WwiseBankLoadHandler : MonoBehaviour
    {
        public static WwiseBankLoadHandler Instance;
        string currentLevelBank;

        private void Awake()
        {
            Instance = this;
        }

        public void PlayEvent(AK.Wwise.Event eventToPlay, GameObject origin)
        {
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