using LW.Audio;
using NaughtyAttributes;
using UnityEngine;

public class WwiseDistanceToWater : MonoBehaviour
{
    const string RTPC_NAME = "GP_OC_WaterHigh";
    float waterPos;
    float masWaterDist;

    [SerializeField] bool byPass = false;
    [SerializeField, MinMaxSlider(0f,1f)] float tktttt;

    public void Init(float waterPosition, float maxDistanceToWater)
    {
        waterPos = waterPosition;
        masWaterDist = maxDistanceToWater;
    }

    private void Update()
    {
        float waterDistanceRatio = Mathf.InverseLerp(waterPos, masWaterDist, transform.position.y);
        if (byPass)
            waterDistanceRatio = tktttt;
        WwiseInterface.Instance.UpdateRTPC(RTPC_NAME, waterDistanceRatio);
    }
}
