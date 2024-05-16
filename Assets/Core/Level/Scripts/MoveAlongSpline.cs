using SplineMesh;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAlongSpline : MonoBehaviour
{
    [SerializeField] AnimationCurve curve;
    [SerializeField] Spline splineToFollow;

    [SerializeField] float timeToCompleteCircuit;
    [SerializeField] float AAA;
    [SerializeField] float offsetOnCurve;
    [SerializeField] Vector3 posOffset;
    float startTime;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        float t = (Time.time - startTime) % timeToCompleteCircuit;
        float x = (t / timeToCompleteCircuit) % 1;
        PlaceFollower(curve.Evaluate(x));
    }

    private void PlaceFollower(float progress)
    {
        progress = ((progress + offsetOnCurve) % 1) * splineToFollow.Length;
        CurveSample sample = splineToFollow.GetSampleAtDistance(progress);
        transform.position = sample.location + posOffset;
        transform.localRotation = sample.Rotation;
    }
}
