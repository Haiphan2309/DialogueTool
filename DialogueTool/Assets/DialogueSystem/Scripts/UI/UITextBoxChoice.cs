using TMPro;
using UnityEngine;

namespace DialogueSystem
{
    public class UITextBoxChoice : MonoBehaviour
    {
        private RectTransform m_rectTransform;
        [SerializeField] private RectTransform m_choosingIcon;
        [SerializeField] private TMP_Text m_text;
        private DialogueChoice m_choice;

        public void Setup(DialogueChoice choice)
        {
            m_choice = choice;
            m_text.text = choice.Text;
            ActiveChoice(false);
        }

        public void ActiveChoice(bool isActive)
        {
            m_choosingIcon.gameObject.SetActive(isActive);
        }

        public DialogueChoice GetDialogueChoice()
        {
            return m_choice;
        }
    }
}
