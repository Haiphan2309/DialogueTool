using UnityEngine;
using GDC.Managers;
using AudioPlayer;
using GDC.Enums;
using DG.Tweening;
using UnityEngine.UI;

namespace GDC.Common
{
    public class SceneTransition : MonoBehaviour
    {
        //public static readonly int InKeyAnimation = Animator.StringToHash("inRight");
        //public static readonly int OutKeyAnimation = Animator.StringToHash("outRight");
        public static SceneTransition Instance { get; private set; }
        //private Animator m_Animator;
        [SerializeField] RectTransform imageRectTransform, inRect, inUpRect, inDownRect, inLeftRect, inRightRect;

        private const float TRANS_DURATION = 1;
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            //m_Animator = GetComponent<Animator>();
        }
        public void TransitionIn(TransitionType transitionType)
        {
            //SoundManager.Instance.PlaySound(SoundID.TRANSITION_IN);
            //m_Animator.CrossFade(InKeyAnimation, 0, 0);
            imageRectTransform.gameObject.SetActive(true);
            Image image = imageRectTransform.GetComponent<Image>();
            image.color = Color.black;

            if (transitionType == TransitionType.LEFT)
            {
                SoundManager.Instance.PlaySound(SoundID.SFX_TRANSITION_IN);
                imageRectTransform.anchoredPosition = new Vector2(-2009, 0);
                imageRectTransform.DOAnchorPosX(0, TRANS_DURATION);
            }
            else if (transitionType == TransitionType.RIGHT)
            {
                SoundManager.Instance.PlaySound(SoundID.SFX_TRANSITION_IN);
                imageRectTransform.anchoredPosition = new Vector2(2009, 0);
                imageRectTransform.DOAnchorPosX(0, TRANS_DURATION);
            }
            else if (transitionType == TransitionType.UP)
            {
                SoundManager.Instance.PlaySound(SoundID.SFX_TRANSITION_IN);
                imageRectTransform.anchoredPosition = new Vector2(0, 1500);
                imageRectTransform.DOAnchorPosY(0, TRANS_DURATION);
            }
            else if (transitionType == TransitionType.DOWN)
            {
                SoundManager.Instance.PlaySound(SoundID.SFX_TRANSITION_IN);
                imageRectTransform.anchoredPosition = new Vector2(0, -1500);
                imageRectTransform.DOAnchorPosY(0, TRANS_DURATION);
            }
            else if (transitionType == TransitionType.IN)
            {
                image.color = Color.clear;
                SoundManager.Instance.PlaySound(SoundID.SFX_TRANSITION_IN);
                inUpRect.anchoredPosition = new Vector2(0, 3000);
                inDownRect.anchoredPosition = new Vector2(0, -3000);
                inLeftRect.anchoredPosition = new Vector2(-3000, 0);
                inRightRect.anchoredPosition = new Vector2(3000, 0);

                inUpRect.DOAnchorPos(new Vector2(0, 900), TRANS_DURATION);
                inDownRect.DOAnchorPos(new Vector2(0, -900), TRANS_DURATION);
                inLeftRect.DOAnchorPos(new Vector2(-900, 0), TRANS_DURATION);
                inRightRect.DOAnchorPos(new Vector2(900, 0), TRANS_DURATION);
                inRect.DORotate(new Vector3(0, 0, 360), TRANS_DURATION, RotateMode.FastBeyond360);
            }
            else if (transitionType == TransitionType.FADE)
            {
                image.color = Color.clear;
                //SoundManager.Instance.PlaySound(SoundID.SFX_LIGHT_TRANSITION);
                imageRectTransform.anchoredPosition = Vector2.zero;
                image.color = Color.clear;
                image.DOColor(Color.white, TRANS_DURATION);
            }
        }
        public void TransitionOut(TransitionType transitionType)
        {
            //SoundManager.Instance.PlaySound(SoundID.TRANSITION_OUT);
            //m_Animator.CrossFade(OutKeyAnimation, 0, 0);
            Image image = imageRectTransform.GetComponent<Image>();
            image.color = Color.black;

            if (transitionType == TransitionType.LEFT)
            {
                SoundManager.Instance.PlaySound(SoundID.SFX_TRANSITION_OUT);
                imageRectTransform.anchoredPosition = new Vector2(0, 0);
                imageRectTransform.DOAnchorPosX(2009, TRANS_DURATION).OnComplete(() => imageRectTransform.gameObject.SetActive(false));
            }
            else if (transitionType == TransitionType.RIGHT)
            {
                SoundManager.Instance.PlaySound(SoundID.SFX_TRANSITION_OUT);
                imageRectTransform.anchoredPosition = new Vector2(0, 0);
                imageRectTransform.DOAnchorPosX(-2009, TRANS_DURATION).OnComplete(() => imageRectTransform.gameObject.SetActive(false));
            }
            else if (transitionType == TransitionType.UP)
            {
                SoundManager.Instance.PlaySound(SoundID.SFX_TRANSITION_OUT);
                imageRectTransform.anchoredPosition = new Vector2(0, 0);
                imageRectTransform.DOAnchorPosY(-1500, TRANS_DURATION).OnComplete(() => imageRectTransform.gameObject.SetActive(false));
            }
            else if (transitionType == TransitionType.DOWN)
            {
                SoundManager.Instance.PlaySound(SoundID.SFX_TRANSITION_OUT);
                imageRectTransform.anchoredPosition = new Vector2(0, 0);
                imageRectTransform.DOAnchorPosY(1500, TRANS_DURATION).OnComplete(() => imageRectTransform.gameObject.SetActive(false));
            }
            else if (transitionType == TransitionType.IN)
            {
                imageRectTransform.gameObject.SetActive(false);
                image.color = Color.clear;
                SoundManager.Instance.PlaySound(SoundID.SFX_TRANSITION_OUT);
                inUpRect.anchoredPosition = new Vector2(0, 900);
                inDownRect.anchoredPosition = new Vector2(0, -900);
                inLeftRect.anchoredPosition = new Vector2(-900, 0);
                inRightRect.anchoredPosition = new Vector2(900, 0);

                inUpRect.DOAnchorPos(new Vector2(0, 3000), TRANS_DURATION);
                inDownRect.DOAnchorPos(new Vector2(0, -3000), TRANS_DURATION);
                inLeftRect.DOAnchorPos(new Vector2(-3000, 0), TRANS_DURATION);
                inRightRect.DOAnchorPos(new Vector2(3000, 0), TRANS_DURATION);
                inRect.DORotate(new Vector3(0, 0, -360), TRANS_DURATION, RotateMode.FastBeyond360);
            }
            else if (transitionType == TransitionType.FADE)
            {
                image.color = Color.clear;
                imageRectTransform.anchoredPosition = Vector2.zero;
                image.color = Color.white;
                image.DOColor(Color.clear, TRANS_DURATION).OnComplete(() => imageRectTransform.gameObject.SetActive(false));
            }
        }
    }
}
