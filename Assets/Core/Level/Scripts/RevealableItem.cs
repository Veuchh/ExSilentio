using LW.Data;
using System;
using UnityEngine;

namespace LW.Level
{
    public class RevealableItem : MonoBehaviour
    {
        [SerializeField] WordID id;
        [SerializeField] Renderer attachedRenderer;

        public WordID ID => id;

        public void Init(WordID newID, Renderer attachedRenderer)
        {
            this.attachedRenderer = attachedRenderer;
            id = newID;
        }

        private void Awake()
        {
            attachedRenderer.enabled = false;
        }

        public void UpdateID(WordID newID)
        {
            id = newID;
        }

        internal void RevealItem()
        {
            attachedRenderer.enabled = true;
        }
    }
}