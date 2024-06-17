using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireLight : MonoBehaviour
{
    [SerializeField] float flickerSpeed = 1;
    [SerializeField] float flickerStrength;
    float baseIntensity;
    Light fireLight;

    private void Awake()
    {
        fireLight = GetComponent<Light>();
        baseIntensity = fireLight.intensity;
    }

    void Update()
    {
        fireLight.intensity =
            Mathf.PerlinNoise(Time.time * flickerSpeed, 0) * flickerStrength + baseIntensity;
    }
}
