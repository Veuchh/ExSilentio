using LW.Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LW.Player
{
    public class PlayerInputsHandler : MonoBehaviour
    {
        public static PlayerInputsHandler Instance;

        WordInputManager wordManager;
        PlayerMovement playerMovement;
        bool isWordModeEnabled = false;

        private void Awake()
        {
            wordManager = GetComponent<WordInputManager>();
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

            playerMovement.OnNewLookInput(value.Get<Vector2>());
        }

        public void OnToggleWordMode(InputValue value)
        {
            ToggleWordMode();
        }

        private void OnNewCharacter(char chr)
        {
            wordManager.AddCharacter(chr);
        }

        public void OnSubmitWord(InputValue value)
        {
            wordManager.SubmitWord();
        }

        public void OnStartWordMode(InputValue value)
        {
             if (isWordModeEnabled)
                return;

            ToggleWordMode();
        }

        public void OnBackspace(InputValue value)
        {
            wordManager.RemoveCharacter();
        }

        void ToggleWordMode()
        {
            isWordModeEnabled = !isWordModeEnabled;

            wordManager.ToggleConsole(isWordModeEnabled);
            Debug.Log((isWordModeEnabled ? "Enabling" : "Disabling ") + "WordMode");

            if (isWordModeEnabled)
            {
                Keyboard.current.onTextInput += OnNewCharacter;
                wordManager.ClearWord();
            }
            else
            {
                Keyboard.current.onTextInput -= OnNewCharacter;
                wordManager.ClearWord();
            }

            if (playerMovement != null)
                PlayerData.IsWordModeEnabled = isWordModeEnabled;
        }
    }
}