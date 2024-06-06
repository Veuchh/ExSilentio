using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BlinkLastCharacter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] float blinkSpeed = 1;

    void Update()
    {
        text.maxVisibleCharacters = text.text.Length -
            (Mathf.Sin(Time.time * blinkSpeed) > 0 ? 1 : 0);
    }
}
