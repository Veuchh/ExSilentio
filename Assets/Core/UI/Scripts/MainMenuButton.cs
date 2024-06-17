using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class MainMenuButton : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image buttonGraphics;
    [SerializeField] TextMeshProUGUI backGroundText;
    [SerializeField] TextMeshProUGUI frontText;
    [SerializeField] LocalizeStringEvent localizeEvent;
    [SerializeField] Sprite defaultSprite;
    [SerializeField] Sprite selectedSprite;

    bool isHighlighted = false;

    private void Awake()
    {
        localizeEvent.OnUpdateString.AddListener(UpdateBGText);
    }

    private void Start()
    {
        UpdateBGText(frontText.text);
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

    void HighlightButton(bool hightLight)
    {
        if (isHighlighted == hightLight)
            return;

        isHighlighted = hightLight;

        backGroundText.text = backGroundText.text.Replace(">  ", (hightLight ? ">" : ">    "));

        buttonGraphics.sprite = hightLight ? selectedSprite : defaultSprite;
        frontText.color = hightLight ? Color.black : Color.white;
        backGroundText.color = hightLight ? Color.black : Color.white;
    }
}