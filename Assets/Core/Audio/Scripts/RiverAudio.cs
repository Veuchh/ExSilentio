using SplineMesh;
using UnityEngine;

public class RiverAudio : MonoBehaviour
{
    [SerializeField] Spline spline;
    Transform cameraTransform;

    void Update()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        float positionIndex = 0;
        float bestDistance = float.MaxValue;
        for (int i = 0; i < 101; i++)
        {
            float sqrDistance = (spline.GetSampleAtDistance((i / 100f) * spline.Length).location - cameraTransform.position).magnitude;
            if (sqrDistance < bestDistance)
            {
                bestDistance = sqrDistance;
                positionIndex = i;
            }
        }

        transform.position = spline.GetSampleAtDistance((positionIndex / 100f) * spline.Length).location;
    }
}
