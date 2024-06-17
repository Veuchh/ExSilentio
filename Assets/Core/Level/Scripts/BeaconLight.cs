using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeaconLight : MonoBehaviour
{
    [SerializeField] AnimationCurve curve;
    [SerializeField] float blinkingSpeed = 1;
    Light beaconLight;

    float lightIntensity;

    private void Awake()
    {
        beaconLight = GetComponent<Light>();
        lightIntensity = beaconLight.intensity;
    }


    void Update()
    {
        beaconLight.intensity = curve.Evaluate(Time.time * blinkingSpeed) * lightIntensity;
    }
}
