using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MemoryShard : MonoBehaviour
{
    const string PLAYER_TAG = "Player";

    [SerializeField] CanvasGroup textToSpawn;
    [SerializeField] Transform trigger;
    [SerializeField] Transform rotatingArrow;
    [SerializeField] float arrowDefaultRotationSpeed = 60f;
    [SerializeField] float arrowLockDuration = 60f;
    [SerializeField] float durationInColliderToLock = .5f;
    [SerializeField] float textFadeDuration = 1.5f;

    float remainingTimeInCollider;
    bool isLocked = false;

    private void Start()
    {
        remainingTimeInCollider = durationInColliderToLock;
        textToSpawn.alpha = 0;
    }

    private void Update()
    {
        if (isLocked)
            return;

        DelockedBehaviour();
    }

    private void DelockedBehaviour()
    {
        //  rotatingArrow in circle
        rotatingArrow.Rotate(
            Vector3.up,
            arrowDefaultRotationSpeed * Time.deltaTime);
    }

    private void OnTriggerStay(Collider other)
    {
        if (isLocked || !other.CompareTag(PLAYER_TAG))
            return;

        remainingTimeInCollider -= Time.deltaTime;

        if (remainingTimeInCollider < 0)
        {
            //  rotatingArrow towards target
            float lookRotationAngle = Quaternion.LookRotation(
                trigger.transform.position - textToSpawn.transform.position,
                Vector3.up).eulerAngles.y;

            isLocked = true;

            Sequence sequence = DOTween.Sequence();

            sequence.Append(rotatingArrow.transform.DORotate(Vector3.up * lookRotationAngle, arrowLockDuration));
            sequence.Append(textToSpawn.DOFade(1, textFadeDuration));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isLocked || !other.CompareTag(PLAYER_TAG))
            return;
    }
}
