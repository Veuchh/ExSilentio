using LW.Audio;
using LW.Data;
using UnityEngine;
using NaughtyAttributes;
using LW.Logger;

namespace LW.Player
{
    [SelectionBase]
    public class PlayerMovement : MonoBehaviour
    {
        const float DEFAULT_GRAVITY = -.05f;
        const string CONRETE_GROUND_TAG = "Concrete";
        const string WATER_GROUND_TAG = "Water";

        [SerializeField] CharacterController characterController;
        [SerializeField] Transform cameraTransform;

        [Header("Movement Settings")]
        [SerializeField] float movementSpeed = 60;
        [SerializeField] float gravity = -9.81f;
        float currentYVelocity;

        [Header("RotationSettings Settings")]
        [SerializeField] float rotationSpeed = 10;
        [SerializeField, MinMaxSlider(-180, 180)] Vector2 verticalLookClamp;

        [Header("Audio Settings")]
        [SerializeField] AK.Wwise.Event startFootstepEvent;
        [SerializeField] AK.Wwise.Event stopFootstepEvent;
        [SerializeField] AK.Wwise.Switch concreteSwitch;
        [SerializeField] AK.Wwise.Switch waterSwitch;

        bool isFootstepPlaying = false;

        float horizontalRotationMultiplier => 360 / (Mathf.Abs(verticalLookClamp.x) + verticalLookClamp.y);

        private float nextUpdateTime;
        private float updateTick = 1;

        Vector3 startPositon;
        Quaternion startRotation;

        Vector3 storedPosition;

        private void Start()
        {
            startPositon = transform.position;
            startRotation = transform.rotation;

            storedPosition = transform.position;
            PlayerInputsHandler.Instance.SetPlayerMovementScript(this);
            WwiseInterface.Instance.UpdatePlayerCamera(GetComponentInChildren<Camera>().gameObject);

            WordInputManager.onResetPlayerPos += ResetPosition;
        }

        private void OnDestroy()
        {
            WordInputManager.onResetPlayerPos -= ResetPosition;
            WwiseInterface.Instance.RemovePlayerCameraIfCameraIsThis(GetComponentInChildren<Camera>().gameObject);
        }

        private void Update()
        {
            Vector3 movement = Vector3.zero;

            if (PlayerData.CanMove)
            {
                ComputeRotation();
                movement = ComputeMovement();
            }

            ComputeGravity();
            movement.y = currentYVelocity;

            characterController.Move(movement);

            Footsteps(movement);

            //Debug
            CustomLogger.IncrementTraveledDistance((transform.position - storedPosition).magnitude);

            storedPosition = transform.position;

            if (nextUpdateTime < Time.time)
            {
                nextUpdateTime += updateTick;
                CustomLogger.AddToTrajectoryHistory(transform.position);
            }
        }

        public void ResetPosition()
        {
            characterController.enabled = false;

            transform.position = startPositon;
            transform.rotation = startRotation;
            cameraTransform.localRotation = Quaternion.identity;

            characterController.enabled = true;
        }

        private void Footsteps(Vector3 currentMovement)
        {
            Vector2 horizontalMovement = new Vector2(currentMovement.x, currentMovement.z);

            float maxRayDistance = 1f;
            bool isPlayerGrounded = Physics.Raycast(transform.position + Vector3.up * .1f, Vector3.down, out RaycastHit hit, maxRayDistance)
                || characterController.isGrounded;

            //Play footstep event
            if (!isFootstepPlaying && isPlayerGrounded && horizontalMovement.sqrMagnitude > 0)
            {
                WwiseInterface.Instance.PlayEvent(startFootstepEvent, gameObject);
                isFootstepPlaying = true;
            }

            //Stop footstep event
            if (isFootstepPlaying && (horizontalMovement.sqrMagnitude == 0 || !isPlayerGrounded))
            {
                WwiseInterface.Instance.PlayEvent(stopFootstepEvent, gameObject);
                isFootstepPlaying = false;
            }

            //Handle surface type
            if (characterController.isGrounded)
            {
                if (hit.collider == null)
                    return;

                switch (hit.collider.tag)
                {
                    case CONRETE_GROUND_TAG:
                        WwiseInterface.Instance.ChangeSwitch(concreteSwitch, gameObject);
                        break;
                    case WATER_GROUND_TAG:
                        WwiseInterface.Instance.ChangeSwitch(waterSwitch, gameObject);
                        break;
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
            Vector3 playerRotation = new Vector3(0, (PlayerData.CurrentLookInput.x * rotationSpeed / horizontalRotationMultiplier), 0);
            transform.Rotate(playerRotation);
        }

        private void ComputeGravity()
        {
            if (characterController.isGrounded)
            {
                currentYVelocity = DEFAULT_GRAVITY;
            }
            else
            {
                currentYVelocity += gravity * Time.deltaTime;
            }
        }

        private Vector3 ComputeMovement()
        {
            //movment
            Vector3 movement = new Vector3(PlayerData.CurrentMoveInput.x, currentYVelocity, PlayerData.CurrentMoveInput.y) * movementSpeed * Time.deltaTime;

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

        public void SetNewSpeed(int newSpeed)
        {
            movementSpeed = newSpeed;
        }
    }
}