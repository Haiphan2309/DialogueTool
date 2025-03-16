using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; private set; }

    [Header("Popup Manager Element")]
    [SerializeField] private Transform canvasTrans;
    [SerializeField] private Image blackBgPrefab;
    private Stack<Image> blackBgStack = new Stack<Image>();
    private Stack<UIBasePopup> popupStack;
    [SerializeField] private TMP_Text touchOutsideText;

    [Header("Popup prefab")]
    //[SerializeField] private UIAnnounce uiAnnouncePrefab;
    //[SerializeField] private UIReward uiRewardPrefab;
    [SerializeField] private UISetting uiSettingPrefab;
    //[SerializeField] private UIShopManager uiShopManagerPrefab;
    //[SerializeField] private UICredit uiCredit;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        popupStack = new Stack<UIBasePopup>();
        DontDestroyOnLoad(gameObject);
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("AAA");
            GameObject clickedUIObject = GameUtils.GetUIObjectUnderPointer();

            if (clickedUIObject != null && clickedUIObject.CompareTag("UIBlack"))
            {
                TryHideCurrentPopup();
            }
        }
    }

    public UIBasePopup GetCurrentPopup()
    {
        if (popupStack.Count > 0) return null;
        return popupStack.Peek();
    }
    private void TryHideCurrentPopup()
    {
        if (popupStack.Count == 0) return;

        if (popupStack.Peek().isCannotHideOutside) return;
        popupStack.Peek().Hide();
    }

    public void ClosePopup()
    {
        if (popupStack.Count > 0)
        {
            HideBlackBg();
            popupStack.Pop();
            if (popupStack.Count > 0)
            {
                if (popupStack.Peek().isCannotHideOutside)
                {
                    touchOutsideText.gameObject.SetActive(false);
                }
                else
                {
                    touchOutsideText.gameObject.SetActive(true);
                    touchOutsideText.transform.SetAsLastSibling();
                }
            }
            else
            {
                touchOutsideText.gameObject.SetActive(false);
            }
        }
    }
    private void PushStack(UIBasePopup uIBasePopup)
    {
        popupStack.Push(uIBasePopup);
        if (uIBasePopup.isCannotHideOutside)
        {
            touchOutsideText.gameObject.SetActive(false);
        }
        else
        {
            touchOutsideText.gameObject.SetActive(true);
            touchOutsideText.transform.SetAsLastSibling();
        }
    }
    public void ShowBlackBg()
    {
        Image blackBgImage = Instantiate(blackBgPrefab, canvasTrans);
        blackBgImage.color = Color.clear;
        blackBgImage.DOFade(0.5f, 0.3f);
        if (blackBgStack == null)
        {
            blackBgStack = new Stack<Image>();
        }
        blackBgStack.Push(blackBgImage);
    }
    public void HideBlackBg()
    {
        Image blackBgImage = blackBgStack?.Pop();
        blackBgImage.DOFade(0.5f, 0.3f).OnComplete(() => Destroy(blackBgImage.gameObject));
    }

    //Show popup prefabs
    [Button]
    public void ShowSetting()
    {
        ShowBlackBg();
        UISetting uiSetting = Instantiate(uiSettingPrefab, canvasTrans);
        uiSetting.Show();
        PushStack(uiSetting);
    }
}
