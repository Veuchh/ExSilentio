using LW.Data;
using System;
using UnityEngine;
using NaughtyAttributes;

namespace LW.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        const float DEFAULT_GRAVITY = -.5f;
        [SerializeField] CharacterController characterController;
        [SerializeField] Transform cameraTransform;

        [Header("Movement Settings")]
        [SerializeField] float movementSpeed = 60;
        [SerializeField] float gravity = -9.81f;
        Vector3 currentVelocity;

        [Header("RotationSettings Settings")]
        [SerializeField] float rotationSpeed = 10;
        [SerializeField, MinMaxSlider(-180, 180)] Vector2 verticalLookClamp;

        float horizontalRotationMultiplier => 360 / (Mathf.Abs(verticalLookClamp.x) + verticalLookClamp.y);

        private void Start()
        {
            PlayerInputsHandler.Instance?.SetPlayerMovementScript(this);
        }

        private void Update()
        {
            if (!PlayerData.CanMove)
                return;

            HandleRotation();

            Vector3 movement = ComputeMovement();

            currentVelocity = movement;

            characterController.Move(movement);
        }

        private void HandleRotation()
        {
            //cameraMovement
            float newCamXRotation = cameraTransform.localRotation.eulerAngles.x + (PlayerData.CurrentCameraInput.y * Time.deltaTime * rotationSpeed * -1);

            // newCamXRotation = Mathf.Clamp(newCamXRotation, verticalLookClamp.x, verticalLookClamp.y);

            if (newCamXRotation > 180 && newCamXRotation < 360 + verticalLookClamp.x)
            {
                newCamXRotation = 360 + verticalLookClamp.x;
            }

            if (newCamXRotation < 180 && newCamXRotation > verticalLookClamp.y)
            {
                newCamXRotation = verticalLookClamp.y;
            }

            cameraTransform.localRotation = Quaternion.Euler(newCamXRotation, 0, 0);

            //Horizontal rotation
            float newCamYRotation = transform.localRotation.eulerAngles.y + (PlayerData.CurrentCameraInput.y * Time.deltaTime * rotationSpeed * -1);

            Vector3 playerRotation = new Vector3(0, (PlayerData.CurrentCameraInput.x * Time.deltaTime * rotationSpeed / horizontalRotationMultiplier), 0);

            transform.Rotate(playerRotation);
            //playerData.CurrentCameraInput.x;
        }

        private Vector3 ComputeMovement()
        {
            //movment
            Vector3 movement = new Vector3(PlayerData.CurrentMoveInput.x, 0, PlayerData.CurrentMoveInput.y) * movementSpeed * Time.deltaTime;

            //gravity
            if (characterController.isGrounded)
            {
                movement.y = DEFAULT_GRAVITY;
            }
            else
            {
                movement.y = movement.y + (gravity * Time.deltaTime);
            }

            return transform.rotation * movement;
        }

        public void OnNewMoveInput(Vector2 newInput)
        {
            PlayerData.CurrentMoveInput = newInput;
        }

        public void OnNewLookInput(Vector2 newInput)
        {
            PlayerData.CurrentCameraInput = newInput;
        }
    }
}