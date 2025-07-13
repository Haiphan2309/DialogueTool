using DialogueSystem.Data;
using GDC.Managers;
using GDC.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem
{
    public class UIChoosingTextBox : MonoBehaviour
    {
        private RectTransform _rectTransform;
        [SerializeField] private Transform _content;

        private List<UITextBoxChoice> _uiTextBoxChoices;
        private UIDialogueTextBox _uiDialogueTextBox;

        private int _currentChoiceIndex;

        /// <summary>
        /// Setup for text box have NPC attach
        /// </summary>
        /// <param name="choiceDatas"></param>
        /// <param name="uiDialogueTextBox"></param>
        /// <param name="objectWorldPos"></param>
        /// <param name="objectSize"></param>
        /// <param name="isNeedToConvertLocalPos"></param>
        public void Setup(List<DSChoiceData> choiceDatas, UIDialogueTextBox uiDialogueTextBox, Vector3 objectWorldPos, float objectSize, bool isNeedToConvertLocalPos = true)
        {
            _rectTransform = GetComponent<RectTransform>();
            _uiDialogueTextBox = uiDialogueTextBox;

            Vector2 resultPos = Vector2.zero;
            if (TrySetPosition(objectWorldPos, objectSize, out resultPos, isNeedToConvertLocalPos))
            {
                _rectTransform.anchoredPosition = resultPos;
            }
            else
            {
                Debug.LogError("Choosing text box is oversize!");
                Hide();
                return;
            }

            if (_uiTextBoxChoices == null)
            {
                _uiTextBoxChoices = new List<UITextBoxChoice>();
            }
            _uiTextBoxChoices.Clear();
            UIUtils.ClearAllChild(_content);

            foreach (var choiceData in choiceDatas)
            {
                UITextBoxChoice uiTextBoxChoice = Instantiate(DialogueManager.Instance.TextBoxConfig.ChoosingTextBoxConfig.UITextBoxChoicePrefab, _content);
                uiTextBoxChoice.Setup(choiceData);
                _uiTextBoxChoices.Add(uiTextBoxChoice);
            }

            _currentChoiceIndex = 0;
            Show();
            ActiveChoice(_currentChoiceIndex);
        }

        /// <summary>
        /// Setup when text box have no NPC attach
        /// </summary>
        /// <param name="choiceDatas"></param>
        /// <param name="uiDialogueTextBox"></param>
        /// <param name="textBoxAnchorPos"></param>
        public void Setup(List<DSChoiceData> choiceDatas, UIDialogueTextBox uiDialogueTextBox)
        {
            Setup(choiceDatas, uiDialogueTextBox, uiDialogueTextBox.transform.position, 0.0f);
        }

        public void UpdatePos(Vector3 objectWorldPos, float objectSize)
        {
            if (gameObject.activeSelf == false)
            {
                return;
            }

            Vector2 resultPos = Vector2.zero;
            if (TrySetPosition(objectWorldPos, objectSize, out resultPos))
            {
                _rectTransform.anchoredPosition = resultPos;
            }
        }

        public void UpdatePos()
        {
            if (gameObject.activeSelf == false)
            {
                return;
            }

            Vector2 resultPos = Vector2.zero;
            if (TrySetPosition(_uiDialogueTextBox.transform.position, 0.0f, out resultPos))
            {
                _rectTransform.anchoredPosition = resultPos;
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

        private bool TrySetPosition(Vector3 objectWorldPos, float objectSize, out Vector2 resultPos, bool isNeedToConvertLocalPos = true)
        {
            RectTransform dialogueContainerRect = transform.parent.GetComponent<RectTransform>();
            Vector2 objectLocalPos = Vector2.zero;

            if (isNeedToConvertLocalPos)
            {
                if (!UIUtils.ConvertWorldPosToLocalRectPos(objectWorldPos, dialogueContainerRect, out resultPos))
                {
                    return false;
                }

                Vector2 objectScreenPos = (Vector2)Camera.main.WorldToScreenPoint(objectWorldPos);
                // Convert screen position to local position in canvas
                
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    dialogueContainerRect,
                    objectScreenPos,
                    Camera.main,
                    out objectLocalPos
                );
            }
            else
            {
                resultPos = Vector2.zero;
            }

            Vector2 containerSize = dialogueContainerRect.rect.size; //Smaller than canvas size a little
            bool isOverSizeUp = _uiDialogueTextBox.GetComponent<RectTransform>().anchoredPosition.y + _uiDialogueTextBox.GetSize().y / 2 + GetSize().y > containerSize.y / 2;
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

            Vector2 uiDialogueTextBoxRectPos = _uiDialogueTextBox.GetComponent<RectTransform>().anchoredPosition;

            bool isHaveEnoughSpaceBottom = uiDialogueTextBoxRectPos.y - _uiDialogueTextBox.GetSize().y / 2 - minY > GetSize().y;

            if (!isOverSizeRight && isHaveEnoughSpaceBottom)
            {
                resultPos.x = Mathf.Clamp(objectLocalPos.x + objectSize + GetSize().x / 2, minX, maxX);
                resultPos.y = Mathf.Clamp(objectLocalPos.y, minY,  uiDialogueTextBoxRectPos.y - _uiDialogueTextBox.GetSize().y / 2 - GetSize().y / 2);
                return true;
            }

            if (!isOverSizeLeft && isHaveEnoughSpaceBottom)
            {
                resultPos.x = Mathf.Clamp(objectLocalPos.x - objectSize - GetSize().x / 2, minX, maxX);
                resultPos.y = Mathf.Clamp(objectLocalPos.y, minY, uiDialogueTextBoxRectPos.y - _uiDialogueTextBox.GetSize().y / 2 - GetSize().y / 2);
                return true;
            }

            if (!isOverSizeDown)
            {
                resultPos.y = Mathf.Min(objectLocalPos.y - objectSize - GetSize().y / 2, uiDialogueTextBoxRectPos.y - _uiDialogueTextBox.GetSize().y / 2 - GetSize().y / 2);
                resultPos.x = Mathf.Clamp(objectLocalPos.x, minX, maxX);
                return true;
            }

            if (!isOverSizeUp)
            {
                resultPos.y = Mathf.Max(objectLocalPos.y + objectSize + GetSize().y / 2, uiDialogueTextBoxRectPos.y + _uiDialogueTextBox.GetSize().y / 2 + GetSize().y / 2);
                resultPos.x = Mathf.Clamp(objectLocalPos.x, minX, maxX);
                return true;
            }

            return false;
        }

        public Vector2 GetSize()
        {
            return _rectTransform.rect.size;
        }

        public void ActiveUpChoice()
        {
            ActiveChoice(_currentChoiceIndex - 1);
        }

        public void ActiveDownChoice()
        {
            ActiveChoice(_currentChoiceIndex + 1);
        }

        private void ActiveChoice(int index)
        {
            index = (index + _uiTextBoxChoices.Count) % _uiTextBoxChoices.Count; //ensure index is not out of range

            _uiTextBoxChoices[_currentChoiceIndex].ActiveChoice(false);
            _currentChoiceIndex = index;
            _uiTextBoxChoices[index].ActiveChoice(true);
        }

        public DSChoiceData GetCurrentChoiceData()
        {
            return _uiTextBoxChoices[_currentChoiceIndex].GetChoiceData();
        }

        public void OnChooseChoice()
        {
            Destroy(this.gameObject);
        }
    }
}
