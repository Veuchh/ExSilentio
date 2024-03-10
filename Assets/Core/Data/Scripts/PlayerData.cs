using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Data
{
    public class PlayerData
    {
        private Vector2 currentMoveInput;
        private Vector2 currentCameraInput;

        public Vector2 CurrentMoveInput { get => currentMoveInput; set => currentMoveInput = value; }
        public Vector2 CurrentCameraInput { get => currentCameraInput; set => currentCameraInput = value; }
    }
}