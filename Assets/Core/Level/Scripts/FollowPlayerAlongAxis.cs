using LW.Data;
using UnityEngine;

namespace LW.Level
{
    public class FollowPlayerAlongAxis : MonoBehaviour
    {
        [SerializeField] bool followXAxis;
        [SerializeField] bool followYAxis;
        [SerializeField] bool followZAxis;
        [SerializeField] float yOffset = 1.75f;

        Vector3 startPos;

        private void Awake()
        {
            startPos = transform.position;
        }

        void Update()
        {
            Transform playerTransform = StaticData.PlayerTransform;

            if (playerTransform == null)
                return;

            transform.position = new Vector3(
                (followXAxis ? playerTransform.position.x : startPos.x),
                (followYAxis ? playerTransform.position.y + yOffset : startPos.y),
                (followZAxis ? playerTransform.position.z : startPos.z));
        }
    }
}