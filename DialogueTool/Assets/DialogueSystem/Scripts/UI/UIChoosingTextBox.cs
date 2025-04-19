using GDC.Managers;
using GDC.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem
{
    public class UIChoosingTextBox : MonoBehaviour
    {
        private RectTransform m_rectTransform;
        [SerializeField] private Transform m_content;

        private List<UITextBoxChoice> m_uiTextBoxChoices;
        private UIDialogueTextBox m_uiDialogueTextBox;

        private int m_currentChoiceIndex;
        private Vector3 m_objectWorldPos;
        private float m_objectSize;

        public void Setup(List<DialogueChoice> choices , UIDialogueTextBox uiDialogueTextBox, Vector3 objectWorldPos, float objectSize = 150f)
        {
            m_rectTransform = GetComponent<RectTransform>();
            m_uiDialogueTextBox = uiDialogueTextBox;
            m_objectSize = objectSize;

            Vector2 resultPos = Vector2.zero;
            if (TrySetPosition(objectWorldPos, objectSize, out resultPos))
            {
                m_rectTransform.anchoredPosition = resultPos;
            }
            else
            {
                Debug.LogError("Choosing text box is oversize!");
                Hide();
                return;
            }

            if (m_uiTextBoxChoices == null)
            {
                m_uiTextBoxChoices = new List<UITextBoxChoice>();
            }
            m_uiTextBoxChoices.Clear();
            UIUtils.ClearAllChild(m_content);

            foreach (var choice in choices)
            {
                UITextBoxChoice uiTextBoxChoice = Instantiate(ConfigManager.Instance.TextBoxConfig.ChoosingTextBoxConfig.UITextBoxChoicePrefab, m_content);
                uiTextBoxChoice.Setup(choice);
                m_uiTextBoxChoices.Add(uiTextBoxChoice);
            }

            m_currentChoiceIndex = 0;
            Show();
            ActiveChoice(m_currentChoiceIndex);
        }

        public void UpdatePos(Vector3 objectWorldPos)
        {
            if (gameObject.activeSelf == false)
            {
                return;
            }

            Vector2 resultPos = Vector2.zero;
            if (TrySetPosition(objectWorldPos, m_objectSize, out resultPos))
            {
                m_rectTransform.anchoredPosition = resultPos;
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private bool TrySetPosition(Vector3 objectWorldPos, float objectSize, out Vector2 resultPos)
        {
            RectTransform dialogueContainerRect = transform.parent.GetComponent<RectTransform>();
            if (!UIUtils.ConvertWorldPosToLocalRectPos(objectWorldPos, dialogueContainerRect, out resultPos))
            {
                return false;
            }

            Vector2 objectScreenPos = (Vector2)Camera.main.WorldToScreenPoint(objectWorldPos);
            // Convert screen position to local position in canvas
            Vector2 objectLocalPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                dialogueContainerRect,
                objectScreenPos,
                Camera.main,
                out objectLocalPos
            );

            Vector2 containerSize = dialogueContainerRect.rect.size; //Smaller than canvas size a little
            bool isOverSizeUp = m_uiDialogueTextBox.GetComponent<RectTransform>().anchoredPosition.y + m_uiDialogueTextBox.GetSize().y / 2 + GetSize().y > containerSize.y / 2;
            bool isOverSizeDown = objectLocalPos.y - objectSize - GetSize().y < -containerSize.y / 2;
            bool isOverSizeRight = objectLocalPos.x + objectSize + GetSize().x > containerSize.x / 2;
            bool isOverSizeLeft = objectLocalPos.x - objectSize - GetSize().x < -containerSize.x / 2;

            if (isOverSizeUp && isOverSizeDown && isOverSizeLeft && isOverSizeDown
                || GetSize().x > containerSize.x
                || GetSize().y > containerSize.y)
            {
                return false;
            }

            float minX = -containerSize.x / 2 + GetSize().x / 2;
            float maxX = containerSize.x / 2 - GetSize().x / 2;
            float minY = -containerSize.y / 2 + GetSize().y / 2;
            float maxY = containerSize.y / 2 - GetSize().y / 2;

            Vector2 uiDialogueTextBoxRectPos = m_uiDialogueTextBox.GetComponent<RectTransform>().anchoredPosition;

            bool isHaveEnoughSpaceBottom = uiDialogueTextBoxRectPos.y - m_uiDialogueTextBox.GetSize().y / 2 - minY > GetSize().y;

            if (!isOverSizeRight && isHaveEnoughSpaceBottom)
            {
                resultPos.x = objectLocalPos.x + objectSize + GetSize().x / 2;
                resultPos.y = Mathf.Clamp(objectLocalPos.y, minY,  uiDialogueTextBoxRectPos.y - m_uiDialogueTextBox.GetSize().y / 2);
                return true;
            }

            if (!isOverSizeLeft && isHaveEnoughSpaceBottom)
            {
                resultPos.x = objectLocalPos.x - objectSize - GetSize().x / 2;
                resultPos.y = Mathf.Clamp(objectLocalPos.y, minY, uiDialogueTextBoxRectPos.y - m_uiDialogueTextBox.GetSize().y / 2);
                return true;
            }

            if (!isOverSizeDown)
            {
                resultPos.y = objectLocalPos.y - objectSize - GetSize().y / 2;
                resultPos.x = Mathf.Clamp(objectLocalPos.x, minX, maxX);
                return true;
            }

            if (!isOverSizeUp)
            {
                resultPos.y = uiDialogueTextBoxRectPos.y + m_uiDialogueTextBox.GetSize().y / 2 + GetSize().y / 2;
                resultPos.x = Mathf.Clamp(objectLocalPos.x, minX, maxX);
                return true;
            }

            return false;
        }

        public Vector2 GetSize()
        {
            return m_rectTransform.rect.size;
        }

        public void ActiveUpChoice()
        {
            ActiveChoice(m_currentChoiceIndex - 1);
        }

        public void ActiveDownChoice()
        {
            ActiveChoice(m_currentChoiceIndex + 1);
        }

        private void ActiveChoice(int index)
        {
            index = (index + m_uiTextBoxChoices.Count) % m_uiTextBoxChoices.Count; //ensure index is not out of range

            m_uiTextBoxChoices[m_currentChoiceIndex].ActiveChoice(false);
            m_currentChoiceIndex = index;
            m_uiTextBoxChoices[index].ActiveChoice(true);
        }

        public DialogueChoice GetCurrentChoice()
        {
            return m_uiTextBoxChoices[m_currentChoiceIndex].GetDialogueChoice();
        }

        public void OnChooseChoice()
        {
            Hide();
        }
    }
}
