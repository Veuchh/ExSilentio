using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{
    const string PLAYER_TAG = "Player";
    const string ANIMATOR_BOOL = "Opened";

    [SerializeField] Animator animator;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(PLAYER_TAG))
            return;

        animator.SetBool(ANIMATOR_BOOL, true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(PLAYER_TAG))
            return;

        animator.SetBool(ANIMATOR_BOOL, false);
    }
}
