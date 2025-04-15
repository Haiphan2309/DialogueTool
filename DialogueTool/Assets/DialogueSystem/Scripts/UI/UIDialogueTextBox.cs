using GDC.Managers;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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

            //TODO: caculate text length for textbox size

            Vector2 resultPos = Vector2.zero;
            PivotType pivotType = PivotType.NONE;
            bool isPivotOverlapObject = false;
            if (CheckPosition(objectWorldPos, out resultPos, ref pivotType, ref isPivotOverlapObject))
            {
                m_rectTransform.anchoredPosition = resultPos;
                SetTextBoxPivot(objectWorldPos, textBoxType, pivotType, isPivotOverlapObject);
                return;
            }

            Debug.LogError("Text box is oversize!");
        }

        public void SetText(string text)
        {
            m_backText.text = text;
            m_frontText.text = m_backText.text;
        }

        public bool ConvertWorldPosToLocalRectPos(Vector3 worldPos, RectTransform parentRect, out Vector2 localPos)
        {
            localPos = Vector2.zero;

            if (parentRect == null)
            {
                return false;
            }

            Vector2 screenPos = (Vector2)Camera.main.WorldToScreenPoint(worldPos);

            // Convert screen position to local position in canvas
            return RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentRect,
                screenPos,
                Camera.main,
                out localPos
            );
        }

        private bool CheckPosition(Vector3 objectWorldPos, out Vector2 resultPos, ref PivotType pivotType, ref bool isPivotOverlapObject)
        {
            RectTransform dialogueContainerRect = transform.parent.GetComponent<RectTransform>();
            if (!ConvertWorldPosToLocalRectPos(objectWorldPos, dialogueContainerRect, out resultPos))
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

            bool isObjectPassTopBorder = objectLocalPos.y - m_objectSize > dialogueContainerRect.rect.size.y / 2;
            bool isObjectPassLeftBorder = objectLocalPos.x + m_objectSize < -dialogueContainerRect.rect.size.x / 2;
            bool isObjectPassRightBorder = objectLocalPos.x - m_objectSize > dialogueContainerRect.rect.size.x / 2;

            float minX = -containerSize.x / 2 + GetSize().x / 2;
            float maxX = containerSize.x / 2 - GetSize().x / 2;
            float minY = -containerSize.y / 2 + GetSize().y / 2;
            float maxY = containerSize.y / 2 - GetSize().y / 2;

            float pivotSize = ConfigManager.Instance.TextBoxConfig.TextBoxPivotConfig.PivotSize;

            if (!isOverSizeUp && !isObjectPassLeftBorder && !isObjectPassRightBorder)
            {
                isPivotOverlapObject = objectLocalPos.y + m_objectSize + pivotSize + GetSize().y > containerSize.y / 2;
                resultPos.y = objectLocalPos.y + m_objectSize + GetSize().y / 2;
                resultPos.y = Mathf.Max(isPivotOverlapObject ? resultPos.y : resultPos.y + pivotSize, minY);
                resultPos.x = Mathf.Clamp(objectLocalPos.x, minX, maxX);
                pivotType = PivotType.DOWN;
                return true;
            }

            if (!isOverSizeRight && !isObjectPassTopBorder)
            {
                isPivotOverlapObject = objectLocalPos.x + m_objectSize + pivotSize + GetSize().x > containerSize.x / 2;
                resultPos.x = objectLocalPos.x + m_objectSize + GetSize().x / 2;
                resultPos.x = Mathf.Max(isPivotOverlapObject ? resultPos.x : resultPos.x + pivotSize, minX);
                resultPos.y = Mathf.Clamp(objectLocalPos.y, minY, maxY);
                pivotType = PivotType.LEFT;
                return true;
            }

            if (!isOverSizeLeft && !isObjectPassTopBorder)
            {
                isPivotOverlapObject = objectLocalPos.x - m_objectSize - pivotSize - GetSize().x < -containerSize.x / 2;
                resultPos.x = objectLocalPos.x - m_objectSize - GetSize().x / 2;
                resultPos.x = Mathf.Min(isPivotOverlapObject ? resultPos.x : resultPos.x - pivotSize, maxX);
                resultPos.y = Mathf.Clamp(objectLocalPos.y, minY, maxY);
                pivotType = PivotType.RIGHT;
                return true;
            }

            if (!isOverSizeDown)
            {
                isPivotOverlapObject = objectLocalPos.y - m_objectSize - pivotSize - GetSize().y < -containerSize.y / 2;
                resultPos.y = objectLocalPos.y - m_objectSize - GetSize().y / 2;
                resultPos.y = Mathf.Min(isPivotOverlapObject ? resultPos.y : resultPos.y - pivotSize, maxY);
                resultPos.x = Mathf.Clamp(objectLocalPos.x, minX, maxX);
                pivotType = PivotType.UP;
                return true;
            }

            return false;
        }

        private void SetTextBoxPivot(Vector3 objectWorldPos, TextBoxType textBoxType, PivotType pivotType, bool isPivotOverlapObject)
        {
            Vector2 objectLocalPos = Vector2.zero;
            if (!ConvertWorldPosToLocalRectPos(objectWorldPos, m_rectTransform, out objectLocalPos))
            {
                return;
            }

            TextBoxPivotPositionConfig pivotPositionConfig = ConfigManager.Instance.TextBoxConfig.TextBoxPivotConfig.GetTextBoxPivotPositionConfig(m_textBoxType, pivotType);
            m_textBoxPivot.rotation = Quaternion.Euler(0, 0, pivotPositionConfig.DegreeZ);
            m_textBoxPivot.anchorMax = pivotPositionConfig.AnchorMax;
            m_textBoxPivot.anchorMin = pivotPositionConfig.AnchorMin;
            Vector2 clampLocalPos = new Vector2(
                Mathf.Clamp(objectLocalPos.x, -GetSize().x / 2, GetSize().x / 2),
                Mathf.Clamp(objectLocalPos.y, -GetSize().y / 2, GetSize().y / 2)
                );

            bool isActive = true;
            switch (pivotType)
            {
                case PivotType.DOWN:
                case PivotType.UP:
                    if (isPivotOverlapObject)
                    {
                        if (GetSize().x - m_objectSize < ConfigManager.Instance.TextBoxConfig.TextBoxPivotConfig.PivotSize) //mean not have enough size for text box pivot
                        {
                            isActive = false;
                            break;
                        }
                        bool isObjectNearLeft = objectLocalPos.x < 0;
                        clampLocalPos.x = Mathf.Clamp(isObjectNearLeft ? objectLocalPos.x + m_objectSize * 1.2f : objectLocalPos.x - m_objectSize * 1.2f, -GetSize().x / 2, GetSize().x / 2);
                        m_textBoxPivot.rotation = Quaternion.Euler(0, isObjectNearLeft ? 0 : 180, m_textBoxPivot.rotation.eulerAngles.z); //flip the lean sprite if need
                    }
                    m_textBoxPivot.anchoredPosition = new Vector2(clampLocalPos.x, pivotPositionConfig.AnchorPos.y);
                    break;

                case PivotType.LEFT:
                case PivotType.RIGHT:
                    if (isPivotOverlapObject)
                    {
                        if (GetSize().y - m_objectSize < ConfigManager.Instance.TextBoxConfig.TextBoxPivotConfig.PivotSize) //mean not have enough size for text box pivot
                        {
                            isActive = false;
                            break;
                        }
                        bool isObjectNearDown = objectLocalPos.x < 0;
                        clampLocalPos.y = Mathf.Clamp(isObjectNearDown ? objectLocalPos.y + m_objectSize * 1.2f : objectLocalPos.y - m_objectSize * 1.2f, -GetSize().y / 2, GetSize().y / 2);
                        m_textBoxPivot.rotation = Quaternion.Euler(0, isObjectNearDown ? 0 : 180, m_textBoxPivot.rotation.eulerAngles.z); //flip the lean sprite if need
                    }
                    m_textBoxPivot.anchoredPosition = new Vector2(pivotPositionConfig.AnchorPos.x, clampLocalPos.y);
                    break;

                default:
                    isActive = false;
                    break;
            }

            m_textBoxPivot.gameObject.SetActive(isActive);

            TextBoxPivotSpriteConfig spriteConfig = ConfigManager.Instance.TextBoxConfig.TextBoxPivotConfig.GetTextBoxPivotSpriteConfig(textBoxType);
            m_textBoxPivot.GetComponent<Image>().sprite = isPivotOverlapObject ? spriteConfig.LeanSprite : spriteConfig.NormalSprite;

            //Vector2 pivotRectPos = m_textBoxPivot.GetComponent<RectTransform>().anchoredPosition;
            //bool isPivotOverlapObject = objectLocalPos.x + m_objectSize / 2 < pivotRectPos.x + ConfigManager.Instance.TextBoxConfig.TextBoxPivotConfig.PivotSize / 2
            //    && objectLocalPos.x - m_objectSize / 2 > pivotRectPos.x - ConfigManager.Instance.TextBoxConfig.TextBoxPivotConfig.PivotSize / 2
            //    && objectLocalPos.y + m_objectSize / 2 < pivotRectPos.y + ConfigManager.Instance.TextBoxConfig.TextBoxPivotConfig.PivotSize / 2
            //    && objectLocalPos.y - m_objectSize / 2 > pivotRectPos.y - ConfigManager.Instance.TextBoxConfig.TextBoxPivotConfig.PivotSize / 2;
        }

        private Vector2 GetSize()
        {
            return m_rectTransform.rect.size;
        }
    }
}
