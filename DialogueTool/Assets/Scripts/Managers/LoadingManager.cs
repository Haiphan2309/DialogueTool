using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GDC.Events;
using UnityEngine.UI;

namespace GDC.Managers
{
    public class LoadingManager : MonoBehaviour
    {
        public static LoadingManager Instance { get; private set; }
        [SerializeField] GameObject loadingCanvas;
        [SerializeField] Image loadingIcon;
        [SerializeField] List<Sprite> loadingSprites;
        [SerializeField] TipConfig tipConfig;
        [SerializeField] TMP_Text loadingText, tipText;
        //[SerializeField] LanguageDictionary loadingDict, tipDict;
        List<TipData> tipDatas;
        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        // Start is called before the first frame update
        void Start()
        {
            GameEvents.ON_LOADING += HandleLoading;
        }
        void OnDestroy()
        {
            GameEvents.ON_LOADING -= HandleLoading;
        }
        void HandleLoading(bool isLoading)
        {
            if (isLoading)
                Debug.Log("start loading");
            else Debug.Log("end loading");
            this.loadingCanvas.SetActive(isLoading);

            if (isLoading)
            {
                int rand = Random.Range(0, loadingSprites.Count);
                loadingIcon.sprite = loadingSprites[rand];
                loadingIcon.SetNativeSize();

                //loadingText.text = loadingDict[SaveLoadManager.Instance.GameData.language];

                if (tipConfig != null && tipConfig.tipDatas.Count > 0)
                {
                    ShowTip();
                }
                else
                {
                    tipText.gameObject.SetActive(false);
                }
            }
        }
        private void ShowTip()
        {
            tipText.gameObject.SetActive(true);
            if (tipDatas == null)
            {
                tipDatas = new List<TipData>();
            }
            tipDatas.Clear();
            foreach (var tipData in tipConfig.tipDatas)
            {
                tipDatas.Add(tipData);
            }

            int rand = Random.Range(0, tipDatas.Count);
            tipText.text = "Tip: " + tipDatas[rand].tip;
        }
    }
}
