using TMPro;
using UnityEngine;

namespace DialogueSystem
{
    public class UITextBoxChoice : MonoBehaviour
    {
        private RectTransform _rectTransform;
        [SerializeField] private RectTransform _choosingIcon;
        [SerializeField] private TMP_Text _text;
        private DialogueChoice _choice;

        public void Setup(DialogueChoice choice)
        {
            _choice = choice;
            _text.text = choice.Text;
            ActiveChoice(false);
        }

        public void ActiveChoice(bool isActive)
        {
            _choosingIcon.gameObject.SetActive(isActive);
        }

        public DialogueChoice GetDialogueChoice()
        {
            return _choice;
        }
    }
}
