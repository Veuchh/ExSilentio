using LW.Audio;
using NaughtyAttributes;
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
    [SerializeField] bool playEvent = false;
    [SerializeField, ShowIf(nameof(playEvent))] AK.Wwise.Event trainStationEnter;
    [SerializeField, ShowIf(nameof(playEvent))] AK.Wwise.Event trainStationExit;
    [SerializeField, ShowIf(nameof(playEvent))] Vector2 trainStationRange;
    bool isInTrainStation;
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

        if (!playEvent)
            return;

        if (!isInTrainStation && x > trainStationRange.x && x < trainStationRange.y)
        {
            isInTrainStation = true;
            WwiseInterface.Instance.PlayEvent(trainStationEnter);
        }
        else if(isInTrainStation && x > trainStationRange.y)
        {
            isInTrainStation = false;
            WwiseInterface.Instance.PlayEvent(trainStationEnter);
        }
    }

    private void PlaceFollower(float progress)
    {
        progress = ((progress + offsetOnCurve) % 1) * splineToFollow.Length;
        CurveSample sample = splineToFollow.GetSampleAtDistance(progress);
        transform.position = sample.location + posOffset;
        transform.localRotation = sample.Rotation;
    }
}
