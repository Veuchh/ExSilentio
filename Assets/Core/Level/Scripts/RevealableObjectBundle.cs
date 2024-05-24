using LW.Audio;
using LW.Data;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Serialization;
using UnityEngine.VFX;

namespace LW.Level
{
    [Serializable]
    public class RevealableObjectBundle : MonoBehaviour
    {
        [SerializeField, FormerlySerializedAs("itemsRevealed")] List<RevealableItem> itemsInBundle;
        [SerializeField] WordID awaitedID;
        [SerializeField] ObjectImportance objectImportance;
        [SerializeField] bool playWwiseEventOnReveal = false;
        [SerializeField] bool isTextureVertical = false;
        [SerializeField, ShowIf(nameof(playWwiseEventOnReveal))] AK.Wwise.Event eventToPlay;
        [SerializeField] HintDatabase hintDatabse;
        [SerializeField, HideInInspector] List<string> hintsBase;

        bool isRevealed = false;
        WordDatabaseEntry entry;
        Dictionary<string, bool> hints = new Dictionary<string, bool>();


        public List<string> HintsBase
        {
            get {
                if (hintsBase == null) hintsBase = new List<string>();
                return hintsBase; 
            }
        }
        public Dictionary<string, bool> Hints => hints;
        public WordID ID => awaitedID;
        public ObjectImportance ObjectImportance => objectImportance;
        public bool IsRevealed => isRevealed;
        public WordDatabaseEntry Entry => entry;

        private void Start()
        {
            RevealableObjectHandler.Instance.RegisterBundle(this);

            foreach (string hintID in HintsBase)
                hints.Add(hintID, false);
        }

        public void RevealBundle(WordID usedID, LocalizedStringTable stringTable)
        {
            Dictionary<string, Texture2D> textureMaps = new Dictionary<string, Texture2D>();
            //Get texture from input
            Texture2D wordTexture = StringToTextureConverter.Instance.GetTextureFromInput(stringTable.GetTable().GetEntry(usedID.ToString()).LocalizedValue);
            textureMaps.Add(
                stringTable.GetTable().GetEntry(usedID.ToString()).LocalizedValue + "_" + (isTextureVertical ? "V" : "H"),
                wordTexture);

            foreach (RevealableItem revealableItem in itemsInBundle)
            {
                //if the item id is the same as the expected but the used ID is different from the expect one, we change it
                if (revealableItem.ID == awaitedID)
                {
                    revealableItem.UpdateID(usedID);
                }

                string usedInput = stringTable.GetTable().GetEntry(revealableItem.ID.ToString()).LocalizedValue;

                if (!textureMaps.Keys.Contains(usedInput + "_" + (revealableItem.IsTextureVertical ? "V" : "H")))
                {
                    textureMaps.Add(usedInput + "_" + (revealableItem.IsTextureVertical ? "V" : "H"), StringToTextureConverter.Instance.GetTextureFromInput(usedInput, isVertical: revealableItem.IsTextureVertical));
                }

                revealableItem.RevealItem(textureMaps[usedInput + "_" + (revealableItem.IsTextureVertical ? "V" : "H")], usedInput.Length);
            }

            if (playWwiseEventOnReveal)
                WwiseInterface.Instance.PlayEvent(eventToPlay);

            isRevealed = true;
        }

        public void SetupElements()
        {
            itemsInBundle = new List<RevealableItem>();


            //Handles cases for the mesh renderers
            foreach (MeshRenderer renderer in GetComponentsInChildren<MeshRenderer>())
            {
                RevealableItem reveleableItem = renderer.GetComponent<RevealableItem>();

                if (reveleableItem == null)
                {
                    reveleableItem = renderer.gameObject.AddComponent<RevealableItem>();
                    reveleableItem.Init(awaitedID, renderer);
                }

                itemsInBundle.Add(reveleableItem);
            }

            //Handles cases for VFX Graphs
            foreach (VisualEffect vfx in GetComponentsInChildren<VisualEffect>())
            {
                RevealableItem reveleableItem = vfx.GetComponent<RevealableItem>();

                if (reveleableItem == null)
                {
                    reveleableItem = vfx.gameObject.AddComponent<RevealableItem>();
                    reveleableItem.Init(awaitedID, vfx, isTextureVertical);
                }

                itemsInBundle.Add(reveleableItem);
            }
        }

        public void Init(WordDatabaseEntry assignedEntry)
        {
            entry = assignedEntry;
        }

        public void RevealHint()
        {
            foreach (string hintID in HintsBase)
            {
                if (hints[hintID] == false)
                {
                    hints[hintID] = true;
                    break;
                }
            }
        }
    }
}