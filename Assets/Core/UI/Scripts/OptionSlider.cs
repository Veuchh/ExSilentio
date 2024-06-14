using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LW.UI
{
    public class OptionSlider : OptionBase
    {
        [SerializeField] Slider slider;
        [SerializeField] TextMeshProUGUI sliderValueLabel;

        public float Value => slider.value;

        public void Init(float newValue)
        {
            slider.value = newValue;
            OnValueChanged();
        }

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

            base.OnValueChanged();
        }

        protected override void ToggleHighlight(bool ishighlighted)
        {
            //TODO : change label & slider color
            base.ToggleHighlight(ishighlighted);
        }
    }
}