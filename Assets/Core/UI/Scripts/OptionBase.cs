using TMPro;
using UnityEngine;

namespace LW.UI
{
    public abstract class OptionBase : MonoBehaviour
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
    }
}