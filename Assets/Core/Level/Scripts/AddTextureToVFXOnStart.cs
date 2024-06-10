using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.VFX;

namespace LW.Level
{
    public class AddTextureToVFXOnStart : MonoBehaviour
    {
        const string TEXTURE_PARAMETER_NAME = "_Main_Tex";
        [SerializeField] LocalizedStringTable stringTable;
        [SerializeField] string entryToTranslate;
        [SerializeField] VisualEffect vfx;

        void Start()
        {
            StartCoroutine(StartRoutine());
        }

        IEnumerator StartRoutine()
        {
            yield return null;
            string input = stringTable.GetTable().GetEntry(entryToTranslate).LocalizedValue;
            var texture = StringToTextureConverter.Instance.GetTextureFromInput(input);
            vfx.SetTexture(TEXTURE_PARAMETER_NAME, texture);
        }
    }
}