using DG.Tweening;
using GDC.Managers;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBasePopup : MonoBehaviour
{
    [SerializeField] private RectTransform panelRect;
    //[SerializeField] List<Image> images = new List<Image>();
    private List<Color> imageOriginColor, textOriginColor, tmpTextOriginColor;
    private List<Button> disableButtons;
    public bool isCannotHideOutside; //Check if popup can be hide when touch outside the popup

    public virtual void Show()
    {
        Show(true);
    }

    public virtual void Show(bool isPlaySound = true)
    {
        ReloadOriginColor();
        if (isPlaySound)
            SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_UI_SHOW);
        DisableAllButton();

        DOTween.Kill(panelRect);

        panelRect.localScale = Vector2.zero;

        Image[] images = panelRect.GetComponentsInChildren<Image>();
        if (images != null)
        {
            for (int i = 0; i < images.Length; i++)
            {
                DOTween.Kill(images[i]);

                Color colorClear = images[i].color;
                colorClear.a = 0;
                images[i].color = colorClear;
                images[i].DOFade(imageOriginColor[i].a, 0.5f);
            }
        }

        Text[] texts = panelRect.GetComponentsInChildren<Text>();
        if (texts != null)
        {
            for (int i = 0; i < texts.Length; i++)
            {
                DOTween.Kill(texts[i]);

                Color colorClear = texts[i].color;
                colorClear.a = 0;
                texts[i].color = colorClear;
                texts[i].DOFade(textOriginColor[i].a, 0.5f);
            }
        }

        TMP_Text[] tmpTexts = panelRect.GetComponentsInChildren<TMP_Text>();
        if (tmpTexts != null)
        {
            for (int i = 0; i < tmpTexts.Length; i++)
            {
                DOTween.Kill(tmpTexts[i]);

                Color colorClear = tmpTexts[i].color;
                colorClear.a = 0;
                tmpTexts[i].color = colorClear;
                tmpTexts[i].DOFade(tmpTextOriginColor[i].a, 0.5f);
            }
        }

        panelRect.DOScale(1, 0.5f).SetEase(Ease.OutBack).OnComplete(() => EnableAllButton());
    }

    public virtual void Hide()
    {
        ReloadOriginColor();
        PopupManager.Instance.ClosePopup();
        DisableAllButton();
        SoundManager.Instance.PlaySound(AudioPlayer.SoundID.SFX_BUTTON_CLICK);

        DOTween.Kill(panelRect);

        Image[] images = panelRect.GetComponentsInChildren<Image>();
        if (images != null)
        {
            foreach (var image in images)
            {
                DOTween.Kill(image);
                image.DOFade(0, 0.5f);
            }
        }

        Text[] texts = panelRect.GetComponentsInChildren<Text>();
        if (texts != null)
        {
            foreach (var text in texts)
            {
                DOTween.Kill(text);
                text.DOFade(0, 0.5f);
            }
        }

        TMP_Text[] tmpTexts = panelRect.GetComponentsInChildren<TMP_Text>();
        if (tmpTexts != null)
        {
            foreach (var text in tmpTexts)
            {
                DOTween.Kill(text);
                text.DOFade(0, 0.5f);
            }
        }
        panelRect.DOScale(0, 0.5f).SetEase(Ease.InBack);
        Destroy(gameObject, 1);
    }

    public void HideNoAnim()
    {
        panelRect.localScale = Vector2.zero;
    }

    private void ReloadOriginImageColor()
    {
        if (imageOriginColor == null) imageOriginColor = new List<Color>();
        imageOriginColor.Clear();
        Image[] images = panelRect.GetComponentsInChildren<Image>();
        foreach (var image in images)
        {
            imageOriginColor.Add(image.color);
        }
    }
    private void ReloadOriginTextColor()
    {
        if (textOriginColor == null) textOriginColor = new List<Color>();
        textOriginColor.Clear();
        Text[] texts = panelRect.GetComponentsInChildren<Text>();
        foreach (var text in texts)
        {
            textOriginColor.Add(text.color);
        }
    }
    private void ReloadOriginTextMeshColor()
    {
        if (tmpTextOriginColor == null) tmpTextOriginColor = new List<Color>();
        tmpTextOriginColor.Clear();
        TMP_Text[] tmpTexts = panelRect.GetComponentsInChildren<TMP_Text>();
        foreach (var text in tmpTexts)
        {
            tmpTextOriginColor.Add(text.color);
        }
    }
    private void ReloadOriginColor()
    {
        ReloadOriginImageColor();
        ReloadOriginTextColor();
        ReloadOriginTextMeshColor();
    }
    public void AddButtonDisable(Button button)
    {
        if (disableButtons == null) disableButtons = new List<Button>();
        disableButtons.Add(button);
    }
    public void ClearButtonDisable()
    {
        if (disableButtons == null) disableButtons = new List<Button>();
        disableButtons.Clear();
    }
    void DisableAllButton()
    {
        Button[] buttons = panelRect.GetComponentsInChildren<Button>();
        if (buttons != null)
        {
            foreach (var button in buttons)
            {
                button.interactable = false;
            }
        }
    }
    void EnableAllButton()
    {
        if (disableButtons == null) disableButtons = new List<Button>();
        Button[] buttons = panelRect.GetComponentsInChildren<Button>();
        if (buttons != null)
        {
            foreach (var button in buttons)
            {
                bool isEnableButton = true;
                foreach (var disableButton in disableButtons)
                {
                    if (disableButton.GetInstanceID() == button.GetInstanceID())
                    {
                        isEnableButton = false;
                        button.interactable = false;
                        break;
                    }
                }
                if (isEnableButton)
                {
                    button.interactable = true;
                }
            }
        }
    }
    //void KillThisDotween()
    //{
    //    DOTween.Kill(panelRect);
    //    foreach (Image image in images)
    //    {
    //        DOTween.Kill(image);
    //    }
    //}
}
