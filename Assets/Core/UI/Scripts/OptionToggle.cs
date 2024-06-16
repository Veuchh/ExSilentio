using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LW.UI
{
    public class OptionToggle : OptionBase
    {
        [SerializeField] Toggle toggle;

        public bool isToggled => toggle.isOn;

        protected override void Awake()
        {
            base.Awake();
            toggle.onValueChanged.AddListener((_) => OnValueChanged());
        }

        private void OnDestroy()
        {
            toggle.onValueChanged.RemoveAllListeners();
        }

        public void Init(bool isToggled)
        {
            toggle.isOn = isToggled;
        }
    }
}