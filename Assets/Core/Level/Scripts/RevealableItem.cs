using LW.Data;
using System;
using UnityEngine;
using UnityEngine.VFX;

namespace LW.Level
{
    public class RevealableItem : MonoBehaviour
    {
        const string DISSOLVE_PARAMETER_NAME = "_Dissolve_Amount";
        const string LETTER_NUMBER_PARAMETER_NAME = "_NumberOfLetters";
        const string DISTANCE_TO_FLAT_PARAMETER_NAME = "_DistanceToFlat";
        const string ANIM_STRENGTH_PARAMETER_NAME = "_MouvementScale";
        const string TEXTURE_PARAMETER_NAME = "_Main_Tex";
        [SerializeField] WordID id;
        [SerializeField] Renderer attachedRenderer;
        [SerializeField] VisualEffect vfx;
        [SerializeField] AnimationCurve revealCurve = default;
        [SerializeField] float revealDuration = 1;
        [SerializeField] bool isTextureVertical;
        [SerializeField] RevealableItemType type;

        MaterialPropertyBlock mpb;

        float startRevealTime;
        float endRevealTime;
        bool isRevealing = false;

        public WordID ID => id;
        public bool IsTextureVertical => isTextureVertical;

        bool wasInitialized = false;

        public void Init(WordID newID, Renderer attachedRenderer)
        {
            this.attachedRenderer = attachedRenderer;
            id = newID;
            revealCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
            type = RevealableItemType.MeshRenderer;
        }

        public void Init(WordID newID, VisualEffect vfx, bool isVertical)
        {
            this.vfx = vfx;
            Debug.Log("VFX " + vfx + " " + this.vfx);
            isTextureVertical = isVertical;
            id = newID;
            revealCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
            type = RevealableItemType.VFXGraph;
        }

        private void Awake()
        {
            switch (type)
            {
                case RevealableItemType.MeshRenderer:
                    MeshRendererAwake();
                    break;
                case RevealableItemType.VFXGraph:
                    VFXGraphAwake();
                    break;
            }
        }

        void MeshRendererAwake()
        {
            wasInitialized = true;
            mpb = new MaterialPropertyBlock();
            attachedRenderer.GetPropertyBlock(mpb);
            mpb.SetFloat(DISSOLVE_PARAMETER_NAME, 0);
            attachedRenderer.SetPropertyBlock(mpb);
        }

        void VFXGraphAwake()
        {
            wasInitialized = true;
            vfx.Stop();
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
            if (!wasInitialized)
            {
                switch (type)
                {
                    case RevealableItemType.MeshRenderer:
                        MeshRendererAwake();
                        break;
                    case RevealableItemType.VFXGraph:
                        VFXGraphAwake();
                        break;
                }
            }

            switch (type)
            {
                case RevealableItemType.MeshRenderer:
                    MeshRendererReveal(texture, letterAmount);
                    break;
                case RevealableItemType.VFXGraph:
                    VFXGraph(texture, letterAmount);
                    break;
            }
        }

        void MeshRendererReveal(Texture2D texture, int letterAmount)
        {
            isRevealing = true;
            attachedRenderer.GetPropertyBlock(mpb);
            mpb.SetTexture(TEXTURE_PARAMETER_NAME, texture);
            mpb.SetInt(LETTER_NUMBER_PARAMETER_NAME, letterAmount);
            attachedRenderer.SetPropertyBlock(mpb);

            startRevealTime = Time.time;
            endRevealTime = Time.time + revealDuration;
        }

        void VFXGraph(Texture2D texture, int letterAmount)
        {
            vfx.Play();
            vfx.SetTexture(TEXTURE_PARAMETER_NAME, texture);
            vfx.SetFloat(LETTER_NUMBER_PARAMETER_NAME, letterAmount);
        }

        public void UpdateSettings(float newAnimationStrength, bool useDistanceToFloat)
        {
            if (type != RevealableItemType.MeshRenderer)
                return;

            if (!wasInitialized)
            {
                switch (type)
                {
                    case RevealableItemType.MeshRenderer:
                        MeshRendererAwake();
                        break;
                    case RevealableItemType.VFXGraph:
                        VFXGraphAwake();
                        break;
                }
            }

            attachedRenderer.GetPropertyBlock(mpb);

            mpb.SetFloat(ANIM_STRENGTH_PARAMETER_NAME, newAnimationStrength);

            mpb.SetFloat(DISTANCE_TO_FLAT_PARAMETER_NAME, useDistanceToFloat ? 1f : 0f);

            attachedRenderer.SetPropertyBlock(mpb);
        }

        public void UpdateDistanceToFlat(bool useDistanceToFloat)
        {
            if (type != RevealableItemType.MeshRenderer)
                return;

            attachedRenderer.GetPropertyBlock(mpb);

            mpb.SetFloat(DISTANCE_TO_FLAT_PARAMETER_NAME, useDistanceToFloat ? 1f : 0f);

            attachedRenderer.SetPropertyBlock(mpb);
        }

        public void UpdateAnimationStrength(float newAnimationStrength)
        {
            if (type != RevealableItemType.MeshRenderer)
                return;

            attachedRenderer.GetPropertyBlock(mpb);

            mpb.SetFloat(ANIM_STRENGTH_PARAMETER_NAME, newAnimationStrength);

            attachedRenderer.SetPropertyBlock(mpb);
        }
    }
}