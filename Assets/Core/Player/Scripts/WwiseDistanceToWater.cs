using LW.Audio;
using UnityEngine;

public class WwiseDistanceToWater : MonoBehaviour
{
    const string RTPC_NAME = "GP_OC_WaterHigh";
    float waterPos;
    float masWaterDist;

    public void Init(float waterPosition, float maxDistanceToWater)
    {
        waterPos = waterPosition;
        masWaterDist = maxDistanceToWater;
    }

    private void Update()
    {
        float waterDistanceRatio = Mathf.InverseLerp(waterPos, masWaterDist, transform.position.y);
        WwiseInterface.Instance.UpdateRTPC(RTPC_NAME, waterDistanceRatio);
    }
}
