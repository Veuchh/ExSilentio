using LW.Audio;
using LW.Data;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.SceneManagement;
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
        [SerializeField] bool revealByDefault;
        [SerializeField] bool playWwiseEventOnReveal = false;
        [SerializeField] bool isTextureVertical = false;
        [SerializeField, ShowIf(nameof(playWwiseEventOnReveal))] AK.Wwise.Event eventToPlay;
        [SerializeField] HintDatabase hintDatabse;
        [SerializeField] UnityEvent OnReveal;
        [SerializeField, HideInInspector] List<string> hintsBase;

        bool isRevealed = false;
        WordDatabaseEntry entry;
        Dictionary<string, bool> hints = new Dictionary<string, bool>();

        public static event Action<RevealableObjectBundle> requestSettings;

        public List<string> HintsBase
        {
            get
            {
                if (hintsBase == null) hintsBase = new List<string>();
                return hintsBase;
            }
        }
        public bool RevealByDefault => revealByDefault;
        public Dictionary<string, bool> Hints => hints;
        public WordID ID => awaitedID;
        public ObjectImportance ObjectImportance => objectImportance;
        public bool IsRevealed => isRevealed;
        public WordDatabaseEntry Entry => entry;

        public string GetBundleKey => SceneManager.GetActiveScene().name + "_" + gameObject.name;

        private void Start()
        {
            StartCoroutine(StartRoutine());
        }

        IEnumerator StartRoutine()
        {
            yield return null;

            RevealableObjectHandler.Instance.RegisterBundle(this, revealByDefault);

            foreach (string hintID in HintsBase)
                hints.Add(hintID, false);

            requestSettings?.Invoke(this);
        }

        public void RevealBundle(WordID usedID, LocalizedStringTable stringTable, bool saveToPlayerPrefs = true)
        {
            Dictionary<string, Texture2D> textureMaps = new Dictionary<string, Texture2D>();

            //Get texture from input
            Texture2D wordTexture = StringToTextureConverter.Instance.GetTextureFromInput(
                stringTable.GetTable().GetEntry(
                    usedID.ToString()).LocalizedValue, 
                    isVertical: isTextureVertical);

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

            if (saveToPlayerPrefs)
                SaveLoad.SaveStringToPlayerPrefs(
                    GetBundleKey,
                    usedID.ToString());

            OnReveal?.Invoke();
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

        public void ApplySettings(float newAnimationStrength, bool useDistanceToFlat)
        {
            foreach (var item in itemsInBundle)
            {
                item.UpdateSettings(newAnimationStrength, useDistanceToFlat);
            }
        }

        public void UpdateDistanceToFlat(bool useDistanceToFloat)
        {
            foreach (var item in itemsInBundle)
            {
                item.UpdateDistanceToFlat(useDistanceToFloat);
            }
        }

        public void UpdateAnimationStrength(float newAnimationStrength)
        {
            foreach (var item in itemsInBundle)
            {
                item.UpdateAnimationStrength(newAnimationStrength);
            }
        }
    }
}