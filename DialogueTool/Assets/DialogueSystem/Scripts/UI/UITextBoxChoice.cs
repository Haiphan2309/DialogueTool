using DialogueSystem.Data;
using TMPro;
using UnityEngine;

namespace DialogueSystem
{
    public class UITextBoxChoice : MonoBehaviour
    {
        private RectTransform _rectTransform;
        [SerializeField] private RectTransform _choosingIcon;
        [SerializeField] private TMP_Text _text;
        private DSChoiceData _choiceData;

        public void Setup(DSChoiceData choiceData)
        {
            _choiceData = choiceData;
            _text.text = choiceData.Text;
            ActiveChoice(false);
        }

        public void ActiveChoice(bool isActive)
        {
            _choosingIcon.gameObject.SetActive(isActive);
        }

        public DSChoiceData GetChoiceData()
        {
            return _choiceData;
        }
    }
}
