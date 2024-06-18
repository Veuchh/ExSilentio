using DG.Tweening;
using LW.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float startupDuration = 2;
    [SerializeField] float fadeInDuration = 5;
    [SerializeField] float stayDuration = 15;
    [SerializeField] float fadeOutDuration = 10;

    bool isEarthRevealed = false;
    bool isPannelOpened = false;
    bool isCreditsStarted = false;

    private void Update()
    {
        if (isEarthRevealed && isPannelOpened && !isCreditsStarted)
        {
            isCreditsStarted = true;

            Sequence sequence = DOTween.Sequence();
            sequence.AppendCallback(() => ConsoleUI.Instance.ToggleConsole(false, true));
            sequence.AppendCallback(() => ConsoleUI.Instance.TogglePreventConsole());
            sequence.AppendInterval(startupDuration);
            sequence.Append(canvasGroup.DOFade(1, fadeInDuration));
            sequence.AppendInterval(stayDuration);
            sequence.Append(canvasGroup.DOFade(0, fadeOutDuration));
            sequence.AppendCallback(() => ConsoleUI.Instance.TogglePreventConsole());
            sequence.AppendCallback(() => ConsoleUI.Instance.ToggleConsole(false, false));
            sequence.Play();
        }
    }

    public void OnEarthRevealed()
    {
        isEarthRevealed = true;
    }

    public void OnPannelOpened()
    {
        isPannelOpened = true;
    }
}
