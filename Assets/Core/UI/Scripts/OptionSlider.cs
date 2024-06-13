using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LW.UI
{
    public class OptionSlider : OptionBase
    {
        [SerializeField] Slider slider;
        [SerializeField] TextMeshProUGUI sliderValueLabel;

        bool isSelected = false;

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