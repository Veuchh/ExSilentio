using LW.Data;
using LW.Logger;
using LW.UI;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LW.Player
{
    public class PlayerInputsHandler : MonoBehaviour
    {
        public static PlayerInputsHandler Instance;

        [SerializeField] bool defaultToLockedCursorMode = true;

        WordInputManager wordManager;
        PlayerMovement playerMovement;
        bool isWordModeEnabled = false;

        [SerializeField] PlayerInput playerInput;

        private InputAction moveAction;
        private InputAction lookAction;
        private InputAction toggleWordModeAction;
        private InputAction submitWordAction;
        private InputAction backspaceAction;
        private InputAction outpoutLogAction;
        private InputAction consoleUpAction;
        private InputAction consoleDownAction;
        private InputAction escapeAction;

        private void Awake()
        {
            ConsoleUI.onCloseClicked += ToggleWordMode;
            wordManager = GetComponent<WordInputManager>();
            Instance = this;
            DontDestroyOnLoad(this);

            SetupInputActions();
        }

        private void Update()
        {

            if ((StaticData.OpenWindowsAmount > 0 && Cursor.lockState == CursorLockMode.Locked)
                || (StaticData.OpenWindowsAmount <= 0 && Cursor.lockState == CursorLockMode.None))
            {
                LockMouse(StaticData.OpenWindowsAmount <= 0);
            }
        }

        private void SetupInputActions()
        {
            moveAction = playerInput.actions["Move"];
            lookAction = playerInput.actions["Look"];
            toggleWordModeAction = playerInput.actions["ToggleWordMode"];
            submitWordAction = playerInput.actions["SubmitWord"];
            backspaceAction = playerInput.actions["Backspace"];
            outpoutLogAction = playerInput.actions["OutputLog"];
            consoleUpAction = playerInput.actions["NavigateConsoleUp"];
            consoleDownAction = playerInput.actions["NavigateConsoleDown"];
            escapeAction = playerInput.actions["Escape"];
        }

        public void LockMouse(bool locked)
        {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
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
            if (isWordModeEnabled || StaticData.OpenWindowsAmount > 0)
                return;

            ToggleWordMode();
        }

        public void OnBackspace(InputValue value)
        {
            if (isWordModeEnabled)
                wordManager.ToggleDelete(value.Get<float>() > .5f);
        }

        public void ToggleWordMode()
        {
            if ((!isWordModeEnabled && StaticData.OpenWindowsAmount > 0)
                || ConsoleUI.Instance.PreventConsole)
                return;

            isWordModeEnabled = !isWordModeEnabled;

            wordManager.ToggleConsole(isWordModeEnabled);
            Debug.Log((isWordModeEnabled ? "Enabling" : "Disabling ") + "WordMode");

            if (isWordModeEnabled)
            {
                CustomLogger.IncrementConsoleOpenValue();
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

        public void OnEscape(InputValue value)
        {
            if (isWordModeEnabled)
            {
                ToggleWordMode();
            }
            else if (MainMenu.Instance.IsPauseMenu && StaticData.OpenWindowsAmount < 2)
            {
                MainMenu.Instance.ToggleMenu();
            }
        }

        public void OnOutputLog(InputValue value)
        {
            CustomLogger.CompileDataToCSV();
        }

        public void OnNavigateConsoleUp(InputValue value)
        {
            wordManager.OnNavigateConsoleHistory(-1);
        }

        public void OnNavigateConsoleDown(InputValue value)
        {
            wordManager.OnNavigateConsoleHistory(1);
        }
    }
}