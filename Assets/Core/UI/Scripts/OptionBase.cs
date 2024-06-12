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
        [SerializeField] string parameterName;

        public string ParameterName => parameterName;

        protected virtual void ToggleHighlight(bool ishighlighted)
        {
            optionName.color = ishighlighted ? highlightedColor : defaultColor;
        }

        protected abstract void OnValueChanged();

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