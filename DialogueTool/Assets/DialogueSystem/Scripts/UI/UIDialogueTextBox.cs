using GDC.Managers;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace DialogueSystem
{
    public class UIDialogueTextBox : MonoBehaviour
    {
        private RectTransform m_rectTransform;
        [SerializeField] private RectTransform m_textBoxPivot;
        [SerializeField] private TMP_Text m_backText, m_frontText;

        [SerializeField] private float m_objectSize = 300;

        [SerializeField] private Transform m_objectTrans;
        [SerializeField] private string m_text;
        [SerializeField] private TextBoxType m_textBoxType;
#if UNITY_EDITOR
        //[Button]
        //public void TestSetup()
        //{
        //    Setup(objectTrans.position);
        //}

        private void Update()
        {
            Setup(m_text, m_textBoxType, m_objectTrans.position);
            m_frontText.text = m_backText.text;
        }
#endif

        public void Setup(string text, TextBoxType textBoxType, Vector3 objectWorldPos)
        {
            m_rectTransform = GetComponent<RectTransform>();

            m_backText.text = text;
            m_frontText.text = m_backText.text;

            Vector2 resultPos = Vector2.zero;
            PivotType pivotType;
            if (CheckPosition(objectWorldPos, out resultPos, out pivotType))
            {
                m_rectTransform.anchoredPosition = resultPos;
                SetTextBoxPivot(objectWorldPos, pivotType);
                return;
            }

            Debug.LogError("Text box is oversize!");
        }

        private bool CheckPosition(Vector3 objectWorldPos, out Vector2 resultPos, out PivotType pivotType)
        {
            RectTransform dialogueContainerRect = transform.parent.GetComponent<RectTransform>();
            Canvas canvas = GetComponentInParent<Canvas>();
            resultPos = Vector2.zero;
            pivotType = PivotType.NONE;
            if (m_rectTransform == null || dialogueContainerRect == null || canvas == null)
            {
                Debug.LogError("Some requirements RectTransform are null!");
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
            bool isOverSizeUp = objectLocalPos.y + m_objectSize + GetSize().y > containerSize.y / 2;
            bool isOverSizeDown = objectLocalPos.y - m_objectSize - GetSize().y < -containerSize.y / 2;
            bool isOverSizeRight = objectLocalPos.x + m_objectSize + GetSize().x > containerSize.x / 2;
            bool isOverSizeLeft = objectLocalPos.x - m_objectSize - GetSize().x < -containerSize.x / 2;

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

            if (!isOverSizeUp)
            {
                resultPos.y = Mathf.Max(objectLocalPos.y + m_objectSize + GetSize().y / 2, minY);
                resultPos.x = Mathf.Clamp(objectLocalPos.x, minX, maxX);
                pivotType = PivotType.DOWN;
                return true;
            }

            if (!isOverSizeRight)
            {
                resultPos.x = Mathf.Max(objectLocalPos.x + m_objectSize + GetSize().x / 2, minX);
                //resultPos.y = Mathf.Clamp(objectLocalPos.y, minY, maxY);
                resultPos.y = maxY;
                pivotType = PivotType.LEFT;
                return true;
            }

            if (!isOverSizeLeft)
            {
                resultPos.x = Mathf.Min(objectLocalPos.x - m_objectSize - GetSize().x / 2, maxX);
                //resultPos.y = Mathf.Clamp(objectLocalPos.y, minY, maxY);
                resultPos.y = maxY;
                pivotType = PivotType.RIGHT;
                return true;
            }

            if (!isOverSizeDown)
            {
                resultPos.y = Mathf.Min(objectLocalPos.y - m_objectSize - GetSize().y / 2, maxY);
                resultPos.x = Mathf.Clamp(objectLocalPos.x, minX, maxX);
                pivotType = PivotType.UP;
                return true;
            }

            return false;
        }

        private void SetTextBoxPivot(Vector3 objectWorldPos, PivotType pivotType)
        {
            Vector2 objectScreenPos = (Vector2)Camera.main.WorldToScreenPoint(objectWorldPos);
            // Convert screen position to local position in canvas
            Vector2 objectLocalPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                m_rectTransform,
                objectScreenPos,
                Camera.main,
                out objectLocalPos
            );

            TextBoxPivotConfig textBoxPivotConfig = ConfigManager.Instance.TextBoxConfig.GetTextBoxPivotConfig(m_textBoxType, pivotType);
            m_textBoxPivot.rotation = Quaternion.Euler(0, 0, textBoxPivotConfig.DegreeZ);
            m_textBoxPivot.anchorMax = textBoxPivotConfig.AnchorMax;
            m_textBoxPivot.anchorMin = textBoxPivotConfig.AnchorMin;
            objectLocalPos = new Vector2(
                Mathf.Clamp(objectLocalPos.x, -GetSize().x / 2, GetSize().x / 2), 
                Mathf.Clamp(objectLocalPos.y, -GetSize().y / 2, GetSize().y / 2)
                );

            switch (pivotType)
            {
                case PivotType.DOWN:
                case PivotType.UP:
                    m_textBoxPivot.gameObject.SetActive(true);
                    m_textBoxPivot.anchoredPosition = new Vector2(objectLocalPos.x, textBoxPivotConfig.AnchorPos.y);
                    break;
                case PivotType.LEFT:
                case PivotType.RIGHT:
                    m_textBoxPivot.gameObject.SetActive(true);
                    m_textBoxPivot.anchoredPosition = new Vector2(textBoxPivotConfig.AnchorPos.x, objectLocalPos.y);
                    break;
                default:
                    m_textBoxPivot.gameObject.SetActive(false);
                    break;
            }
        }

        private Vector2 GetSize()
        {
            return m_rectTransform.rect.size;
        }
    }
}
