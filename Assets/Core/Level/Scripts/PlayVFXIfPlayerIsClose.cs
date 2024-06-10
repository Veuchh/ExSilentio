using LW.Data;
using UnityEngine;
using UnityEngine.VFX;

namespace LW.Level
{
    public class PlayVFXIfPlayerIsClose : MonoBehaviour
    {
        [SerializeField] float maxSquareDistance = 25;
        [SerializeField] VisualEffect vfx;

        bool isPlaying = true;

        void Update()
        {
            Transform playerTransform = StaticData.PlayerTransform;

            if (playerTransform == null)
                return;

            float sqrDistance = Vector3.SqrMagnitude(transform.position - playerTransform.position);

            if (isPlaying && sqrDistance < maxSquareDistance || !isPlaying && sqrDistance >= maxSquareDistance)
                return;

            if (!isPlaying)
                vfx.Play();
            else
                vfx.Stop();

            isPlaying = !isPlaying;
        }
    }
}