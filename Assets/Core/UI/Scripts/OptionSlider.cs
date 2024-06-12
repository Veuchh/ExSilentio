using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LW.UI
{
    public class OptionSlider : OptionBase
    {
        [SerializeField] Slider slider;
        [SerializeField] TextMeshProUGUI sliderValueLabel;

        private void Awake()
        {
            slider.onValueChanged.AddListener((_) => OnValueChanged());
        }

        private void OnDestroy()
        {
            slider.onValueChanged.RemoveAllListeners();
        }

        protected override void OnValueChanged()
        {
            string newText;

            if (slider.wholeNumbers)
                newText = slider.value.ToString();
            else
                newText = slider.value.ToString("F2");

            sliderValueLabel.text = newText;
        }
    }
}