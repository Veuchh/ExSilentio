using LW.Audio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonAudioHandler : MonoBehaviour, ISelectHandler, IPointerEnterHandler
{
    [SerializeField] Button button;
    [SerializeField] AK.Wwise.Event hoverEvent;
    [SerializeField] AK.Wwise.Event clickEvent;

    private void Awake()
    {
        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
    }

    public void OnSelect(BaseEventData eventData)
    {
        PlayHoverEvent();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayHoverEvent();
    }

    private void OnButtonClicked()
    {
        WwiseInterface.Instance.PlayEvent(clickEvent);
    }

    void PlayHoverEvent()
    {
        WwiseInterface.Instance.PlayEvent(hoverEvent);

    }
}
