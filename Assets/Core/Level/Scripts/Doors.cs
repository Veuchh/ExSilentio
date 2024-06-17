using LW.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{
    const string PLAYER_TAG = "Player";
    const string ANIMATOR_BOOL = "Opened";

    [SerializeField] Animator animator;
    [SerializeField] AK.Wwise.Event doorOpen;
    [SerializeField] AK.Wwise.Event doorClose;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(PLAYER_TAG))
            return;

        animator.SetBool(ANIMATOR_BOOL, true);
        WwiseInterface.Instance.PlayEvent(doorOpen, gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(PLAYER_TAG))
            return;

        animator.SetBool(ANIMATOR_BOOL, false);
        WwiseInterface.Instance.PlayEvent(doorClose, gameObject);
    }
}
