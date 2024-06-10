using UnityEngine;

namespace LW.Data
{
    public static class PlayerData
    {
        private static Vector2 currentMoveInput;
        private static Vector2 currentCameraInput;
        private static bool isWordModeEnabled;

        public static Vector2 CurrentMoveInput { get => currentMoveInput; set => currentMoveInput = value; }
        public static Vector2 CurrentLookInput { get => currentCameraInput; set => currentCameraInput = value; }
        public static bool IsWordModeEnabled { get => isWordModeEnabled; set => isWordModeEnabled = value; }

        public static bool CanMove => !isWordModeEnabled && StaticData.OpenWindowsAmount == 0;
    }
}