using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.Localization;
using System.Collections.Generic;
using System.Collections;

public class MemoryShard : MonoBehaviour
{
    const string PLAYER_TAG = "Player";
    const string MEMORYSHARD_ID = "MemoryShard";
    const string TEXTURE_ID = "_Main_Tex";
    const string DISSOLVE_ID = "_Dissolve_Amount";

    [SerializeField] LocalizedStringTable memoryShardTable;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] CanvasGroup textToSpawn;
    [SerializeField] Transform trigger;
    [SerializeField] Transform rotatingArrow;
    [SerializeField] float arrowDefaultRotationSpeed = 60f;
    [SerializeField] float arrowLockDuration = 60f;
    [SerializeField] float durationInColliderToLock = .5f;
    [SerializeField] float textFadeDuration = 1.5f;
    [SerializeField] float textureDissolveTime = .4f;
    [SerializeField] UnityEvent onRevealText;
    [SerializeField] List<Renderer> ringsToDissolve;

    float remainingTimeInCollider;
    float startDissolveTime;
    float endDissolveTime;
    bool isLocked = false;

    bool isFadingRings = false;

    private void Start()
    {
        remainingTimeInCollider = durationInColliderToLock;
        textToSpawn.alpha = 0;

        StartCoroutine(StartRoutine());
    }

    IEnumerator StartRoutine()
    {
        yield return null;
        string textToWrite = memoryShardTable.GetTable().GetEntry(MEMORYSHARD_ID).LocalizedValue;

        Texture2D texture = StringToTextureConverter.Instance.GetTextureFromInput(textToWrite);

        texture.Apply();

        MaterialPropertyBlock mpb = new MaterialPropertyBlock();

        meshRenderer.GetPropertyBlock(mpb);
        mpb.SetTexture(TEXTURE_ID, texture);
        meshRenderer.SetPropertyBlock(mpb);
    }

    private void Update()
    {
        if (isLocked)
        {
            if (isFadingRings)
            {
                MaterialPropertyBlock mpb = new MaterialPropertyBlock();

                float dissolveAmount = Mathf.InverseLerp(startDissolveTime, endDissolveTime, Time.time);

                foreach (var item in ringsToDissolve)
                {
                    item.GetPropertyBlock(mpb);

                    mpb.SetFloat(DISSOLVE_ID, Time.time > endDissolveTime ? 1 : dissolveAmount);

                    item.SetPropertyBlock(mpb);
                }

                if (Time.time > endDissolveTime)
                    isFadingRings = false;
            }

            return;
        }

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
            sequence.AppendCallback(() => onRevealText?.Invoke());
            sequence.AppendCallback(() => StartFadingRings());
            sequence.Append(textToSpawn.DOFade(1, textFadeDuration));
        }
    }

    void StartFadingRings()
    {
        isFadingRings = true;
        startDissolveTime = Time.time;
        endDissolveTime = Time.time + textureDissolveTime;
    }

    private void OnTriggerExit(Collider other)
    {
        if (isLocked || !other.CompareTag(PLAYER_TAG))
            return;
    }
}
