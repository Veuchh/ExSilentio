using LW.Audio;
using LW.Data;
using UnityEngine;
using NaughtyAttributes;
using LW.Logger;
using System;

namespace LW.Player
{
    [SelectionBase]
    public class PlayerMovement : MonoBehaviour
    {
        const float DEFAULT_GRAVITY = -.5f;
        [SerializeField] CharacterController characterController;
        [SerializeField] Transform cameraTransform;

        [Header("Movement Settings")]
        [SerializeField] float movementSpeed = 60;
        [SerializeField] float gravity = -9.81f;

        [Header("RotationSettings Settings")]
        [SerializeField] float rotationSpeed = 10;
        [SerializeField, MinMaxSlider(-180, 180)] Vector2 verticalLookClamp;

        [Header("Audio Settings")]
        [SerializeField] AK.Wwise.Event footstepEvent;
        [SerializeField] AK.Wwise.Switch groundSwitch;
        [SerializeField] AK.Wwise.Switch waterSwitch;
        [SerializeField] float footstepTickDuration = .5f;
        float footstepCoolDown;
        bool isOnGround = true;

        float horizontalRotationMultiplier => 360 / (Mathf.Abs(verticalLookClamp.x) + verticalLookClamp.y);


        private float nextUpdateTime;
        private float updateTick = 1;
        Vector3 storedPosition;

        private void Start()
        {
            storedPosition = transform.position;
            PlayerInputsHandler.Instance.SetPlayerMovementScript(this);
            WwiseInterface.Instance.UpdatePlayerCamera(GetComponentInChildren<Camera>().gameObject);
        }

        private void OnDestroy()
        {
            WwiseInterface.Instance.RemovePlayerCameraIfCameraIsThis(GetComponentInChildren<Camera>().gameObject);
        }

        private void Update()
        {
            if (!PlayerData.CanMove)
                return;

            ComputeRotation();

            Vector3 movement = ComputeMovement();

            characterController.Move(movement);

            CustomLogger.IncrementTraveledDistance((transform.position - storedPosition).magnitude);

            storedPosition = transform.position;

            if (nextUpdateTime < Time.time)
            {
                nextUpdateTime += updateTick;
                CustomLogger.AddToTrajectoryHistory(transform.position);
            }

            Footsteps();
        }

        private void Footsteps()
        {
            //Raycast to check for ground type
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1))
            {
                footstepCoolDown -= Time.deltaTime;
                if (footstepCoolDown <= 0)
                {
                    footstepCoolDown += footstepTickDuration;

                    //TODO : better wauy of checking ground type
                    WwiseInterface.Instance.SetSwitch(hit.collider.CompareTag("Ground") ? groundSwitch : waterSwitch);
                    WwiseInterface.Instance.PlayEvent(footstepEvent);
                }
            }
        }

        private void ComputeRotation()
        {
            //cameraMovement
            float newCamXRotation = cameraTransform.localRotation.eulerAngles.x + (PlayerData.CurrentLookInput.y * rotationSpeed * -1);

            //rotation clamping
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
            //  float newCamYRotation = transform.localRotation.eulerAngles.y + (PlayerData.CurrentLookInput.y * Time.deltaTime * rotationSpeed * -1);
            Vector3 playerRotation = new Vector3(0, (PlayerData.CurrentLookInput.x * rotationSpeed / horizontalRotationMultiplier), 0);
            transform.Rotate(playerRotation);
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

            //Rotates the movement input by the current rotation of the player
            return transform.rotation * movement;
        }

        public void OnNewMoveInput(Vector2 newInput)
        {
            PlayerData.CurrentMoveInput = newInput;
        }

        public void OnNewLookInput(Vector2 newInput)
        {
            PlayerData.CurrentLookInput = newInput;
        }
    }
}