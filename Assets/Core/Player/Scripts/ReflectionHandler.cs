using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectionHandler : MonoBehaviour
{
    const string REFLECTION_POS_PROPERTY_NAME = "_Reflection_Pos";
    const string SUN_TAG = "Sun";

    [SerializeField] List<MeshRenderer> waterPlanes;
    [SerializeField] MaterialPropertyBlock mpb;
    float lerpValue = .2f;
    Vector3 sunPosition;

    public void Init(List<MeshRenderer> waterPlanes)
    {
        this.waterPlanes = waterPlanes;
        sunPosition = GameObject.FindGameObjectWithTag(SUN_TAG).transform.position;
        mpb = new MaterialPropertyBlock();
    }

    private void Update()
    {
        Vector3 reflectionPosition = Vector3.Lerp(transform.position, sunPosition, lerpValue);
        reflectionPosition.y = 0;
        foreach (var item in waterPlanes)
        {
            item.GetPropertyBlock(mpb);
            mpb.SetVector(REFLECTION_POS_PROPERTY_NAME, reflectionPosition);
            item.SetPropertyBlock(mpb);
        }
    }
}
