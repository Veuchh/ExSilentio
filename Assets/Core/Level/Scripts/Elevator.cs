using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace LW.Level
{
    public class Elevator : MonoBehaviour
    {
        const string PLAYER_TAG = "Player";

        [SerializeField] float elevatorDuration = 20;
        [SerializeField] UnityEvent onPlatformMovingUp;
        [SerializeField] UnityEvent onPlatformMovingDown;
        [SerializeField] Transform platform;
        [SerializeField] GameObject sideColliders;
        [SerializeField] Vector3 startPosition;
        [SerializeField] Vector3 endPosition;
        float startTime;
        float endTime;
        bool isMoving = false;
        Vector3 targetPos;

#if UNITY_EDITOR
        [Button]
        void RegisterStartPosition() => startPosition = platform.position;

        [Button]
        void RegisterEndPosition() => endPosition = platform.position;
#endif

        private void FixedUpdate()
        {
            if (!isMoving)
                return;

            if (endTime < Time.time)
            {
                platform.position = targetPos;
                isMoving = false;
                return;
            }

            if (targetPos == endPosition)
                platform.position = Vector3.Lerp(startPosition, endPosition, Mathf.InverseLerp(startTime, endTime, Time.time));
            else
                platform.position = Vector3.Lerp(endPosition, startPosition, Mathf.InverseLerp(startTime, endTime, Time.time));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(PLAYER_TAG))
                return;

            if (platform.position == startPosition && !isMoving)
                targetPos = endPosition;

            else if (platform.position == endPosition && !isMoving)
                targetPos = startPosition;

            else
            {
                Debug.LogError($"A collider ({other.gameObject.name}) with tag {PLAYER_TAG} entered the elevator while it was moving.", gameObject);
                return;
            }

            Transform player = other.transform.parent;

            Sequence sequence = DOTween.Sequence();

            sequence.AppendCallback(() => sideColliders.SetActive(true));
            sequence.AppendCallback(() => player.SetParent(platform));
            sequence.AppendInterval(elevatorDuration / 2);

            if (targetPos == startPosition)
                sequence.AppendCallback(() => onPlatformMovingUp?.Invoke());
            else
                sequence.AppendCallback(() => onPlatformMovingDown?.Invoke());

            sequence.AppendInterval(elevatorDuration / 2);
            sequence.AppendCallback(() => sideColliders.SetActive(false));
            sequence.AppendCallback(() => player.SetParent(null));
            sequence.Play();

            startTime = Time.time;
            endTime = Time.time + elevatorDuration;
            isMoving = true;
        }
    }
}