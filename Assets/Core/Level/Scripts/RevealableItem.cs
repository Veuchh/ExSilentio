using LW.Data;
using UnityEngine;

namespace LW.Level
{
    public class RevealableItem : MonoBehaviour
    {
        const string DISSOLVE_PARAMETER_NAME = "_Dissolve_Amount";
        const string LETTER_NUMBER_PARAMETER_NAME = "_NumberOfLetters";
        const string TEXTURE_PARAMETER_NAME = "_Main_Tex";
        [SerializeField] WordID id;
        [SerializeField] Renderer attachedRenderer;
        [SerializeField] AnimationCurve revealCurve = default;
        [SerializeField] float revealDuration = 1;

        MaterialPropertyBlock mpb;

        float startRevealTime;
        float endRevealTime;
        bool isRevealing = false;

        public WordID ID => id;

        public void Init(WordID newID, Renderer attachedRenderer)
        {
            this.attachedRenderer = attachedRenderer;
            id = newID;
            revealCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

        }

        private void Awake()
        {
            mpb = new MaterialPropertyBlock();
            attachedRenderer.GetPropertyBlock(mpb);

            mpb.SetFloat(DISSOLVE_PARAMETER_NAME, 0);

            attachedRenderer.SetPropertyBlock(mpb);
        }

        private void Update()
        {
            if (!isRevealing)
                return;

            if (Time.time >= endRevealTime)
            {
                mpb.SetFloat(DISSOLVE_PARAMETER_NAME, 1);
                attachedRenderer.SetPropertyBlock(mpb);
                isRevealing = false;
                return;
            }

            mpb.SetFloat(DISSOLVE_PARAMETER_NAME, revealCurve.Evaluate(Mathf.InverseLerp(startRevealTime, endRevealTime, Time.time)));
            attachedRenderer.SetPropertyBlock(mpb);
        }

        public void UpdateID(WordID newID)
        {
            id = newID;
        }

        public void RevealItem(Texture2D texture, int letterAmount)
        {
            isRevealing = true;
            startRevealTime = Time.time;
            endRevealTime = Time.time + revealDuration;
            attachedRenderer.GetPropertyBlock(mpb);
            mpb.SetTexture(TEXTURE_PARAMETER_NAME, texture);
            mpb.SetInt(LETTER_NUMBER_PARAMETER_NAME, letterAmount);
            attachedRenderer.SetPropertyBlock(mpb);
        }
    }
}