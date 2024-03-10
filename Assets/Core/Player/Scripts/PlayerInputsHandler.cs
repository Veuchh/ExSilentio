using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LW.Player
{
    public class PlayerInputsHandler : MonoBehaviour
    {
        public static PlayerInputsHandler Instance;

        PlayerMovement playerMovement;

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }

        public void SetPlayerMovementScript(PlayerMovement playerMovement)
        {
            this.playerMovement = playerMovement;
        }

        public void OnMove(InputValue value)
        {
            if (playerMovement == null)
                return;

            playerMovement.OnNewMoveInput(value.Get<Vector2>().normalized);
        }

        public void OnLook(InputValue value)
        {
            if (playerMovement == null)
                return;

            playerMovement.OnNewLookInput(value.Get<Vector2>().normalized);
        }
    }
}