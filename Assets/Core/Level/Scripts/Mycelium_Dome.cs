using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;
using LW.Audio;

public class Mycelium_Dome : MonoBehaviour
{
    const string PLAYER_TAG = "Player";

    [SerializeField] Transform targetObject;
    [SerializeField] float openingDuration = 5;
    [SerializeField] AnimationCurve rotationCurve;
    [SerializeField] AK.Wwise.Event pannelOpenAudio;

    [SerializeField, HideInInspector] Quaternion endRotation;
    bool isOpened = false;

    private void OnTriggerEnter(Collider other)
    {
        //TODO : this piece of code uses a magic number. In the future, we should have a way to know how many totoal level we have.
        if (isOpened
            || SaveLoad.GetIntFromPlayerPrefs(SaveLoad.SCENE_WITH_ALL_CORE_REVEALED) < 2
            || !other.CompareTag(PLAYER_TAG))
            return;

        isOpened = true;

        WwiseInterface.Instance.PlayEvent(pannelOpenAudio, targetObject.gameObject);
        targetObject.DORotateQuaternion(endRotation, openingDuration).SetEase(rotationCurve);
    }

    [Button]
    void TestRotation ()
    {
        targetObject.DORotateQuaternion(endRotation, openingDuration).SetEase(rotationCurve);
}

#if UNITY_EDITOR
    [Button]
    void SetupTargetValue()
    {
        endRotation = targetObject.rotation;
    }
#endif
}
