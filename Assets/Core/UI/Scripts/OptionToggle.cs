using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LW.UI
{
    public class OptionToggle : OptionBase
    {
        protected override void ToggleHighlight(bool ishighlighted)
        {
            base.ToggleHighlight(ishighlighted);
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