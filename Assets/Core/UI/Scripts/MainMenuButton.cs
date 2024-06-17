using LW.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class MainMenuButton : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Button button;
    [SerializeField] Image buttonGraphics;
    [SerializeField] TextMeshProUGUI backGroundText;
    [SerializeField] TextMeshProUGUI frontText;
    [SerializeField] LocalizeStringEvent localizeEvent;
    [SerializeField] Sprite defaultSprite;
    [SerializeField] Sprite selectedSprite;
    [SerializeField] AK.Wwise.Event highlightEvent;
    [SerializeField] AK.Wwise.Event clickEvent;

    bool isHighlighted = false;

    private void Awake()
    {
        localizeEvent.OnUpdateString.AddListener(UpdateBGText);
    }

    private void Start()
    {
        UpdateBGText(frontText.text);
        button.onClick.AddListener(OnButtonClicked);
    }
    private void OnDestroy()
    {
        localizeEvent.OnUpdateString.RemoveListener(UpdateBGText);
    }

    void UpdateBGText(string foregroundText)
    {
        backGroundText.text = string.Empty;

        foreach (char chr in foregroundText)
        {
            backGroundText.text += ' ';
        }

        backGroundText.text = "> " + backGroundText.text + " <";
    }

    private void OnButtonClicked()
    {
        WwiseInterface.Instance.PlayEvent(clickEvent);
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        HighlightButton(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HighlightButton(false);
    }

    public void OnSelect(BaseEventData eventData)
    {
        HighlightButton(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        HighlightButton(false);
    }

    void HighlightButton(bool highlight)
    {
        if (isHighlighted == highlight)
            return;

        isHighlighted = highlight;

        backGroundText.text = backGroundText.text.Replace(">  ", (highlight ? ">" : ">    "));

        buttonGraphics.sprite = highlight ? selectedSprite : defaultSprite;
        frontText.color = highlight ? Color.black : Color.white;
        backGroundText.color = highlight ? Color.black : Color.white;

        if (highlight)
            WwiseInterface.Instance.PlayEvent(highlightEvent);
    }
}