using LW.Audio;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LW.UI
{
    public abstract class OptionBase : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] Color defaultColor = new Color(1, 1, 1, .8f);
        [SerializeField] Color highlightedColor = new Color(1, 1, 1, 1);
        [SerializeField] TextMeshProUGUI optionName;
        [SerializeField] AK.Wwise.Event hoverEvent;
        [SerializeField] AK.Wwise.Event clickEvent;
        [SerializeField] string parameterName;

        public event Action onValueChanged;

        public string ParameterName => parameterName;

        protected virtual void Awake()
        {
            ToggleHighlight(false);
        }

        protected virtual void ToggleHighlight(bool ishighlighted)
        {
            optionName.color = ishighlighted ? highlightedColor : defaultColor;

            if (ishighlighted)
                WwiseInterface.Instance.PlayEvent(hoverEvent);
        }

        protected virtual void OnValueChanged()
        {
            onValueChanged?.Invoke();
            WwiseInterface.Instance.PlayEvent(clickEvent);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            ToggleHighlight(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ToggleHighlight(false);
        }

        public void OnSelect(BaseEventData eventData)
        {
            ToggleHighlight(true);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            ToggleHighlight(false);
        }
    }
}