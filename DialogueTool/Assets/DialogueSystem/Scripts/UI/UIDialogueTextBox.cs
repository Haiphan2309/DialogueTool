using GDC.Managers;
using GDC.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem
{
    public class UIDialogueTextBox : MonoBehaviour
    {
        private RectTransform m_rectTransform;
        [SerializeField] private RectTransform m_textBoxPivot;
        [SerializeField] private TMP_Text m_text;

        private TextBoxType m_textBoxType;

        public void Setup(string text, TextBoxType textBoxType, Vector3 objectWorldPos, float objectSize)
        {
            m_rectTransform = GetComponent<RectTransform>();
            m_textBoxType = textBoxType;

            SetText(text);

            Vector2 resultPos = Vector2.zero;
            PivotType pivotType = PivotType.NONE;
            bool isPivotOverlapObject = false;
            if (TrySetPosition(objectWorldPos, objectSize, out resultPos, ref pivotType, ref isPivotOverlapObject))
            {
                m_rectTransform.anchoredPosition = resultPos;
                TrySetTextBoxPivot(objectWorldPos, objectSize, textBoxType, pivotType, isPivotOverlapObject);
                Show();
                return;
            }

            Debug.Log("Text box is oversize! can't spawn!");
            Hide();
        }

        public void UpdatePos(Vector3 objectWorldPos, float objectSize)
        {
            if (gameObject.activeSelf == false)
            {
                return;
            }

            Vector2 resultPos = Vector2.zero;
            PivotType pivotType = PivotType.NONE;
            bool isPivotOverlapObject = false;
            if (TrySetPosition(objectWorldPos, objectSize, out resultPos, ref pivotType, ref isPivotOverlapObject))
            {
                m_rectTransform.anchoredPosition = resultPos;
                TrySetTextBoxPivot(objectWorldPos, objectSize, m_textBoxType, pivotType, isPivotOverlapObject);
                Show();
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

        public void SetText(string text)
        {
            m_text.text = text;
        }

        private bool TrySetPosition(Vector3 objectWorldPos, float objectSize, out Vector2 resultPos, ref PivotType pivotType, ref bool isPivotOverlapObject)
        {
            resultPos = Vector2.zero;

            RectTransform dialogueContainerRect = transform.parent.GetComponent<RectTransform>();          
            Vector2 objectLocalPos = Vector2.zero;
            if (!UIUtils.ConvertWorldPosToLocalRectPos(objectWorldPos, dialogueContainerRect, out objectLocalPos))
            {
                return false;
            }

            Vector2 containerSize = dialogueContainerRect.rect.size; //Smaller than canvas size a little
            bool isOverSizeUp = objectLocalPos.y + objectSize + GetSize().y > containerSize.y / 2;
            bool isOverSizeDown = objectLocalPos.y - objectSize - GetSize().y < -containerSize.y / 2;
            bool isOverSizeRight = objectLocalPos.x + objectSize + GetSize().x > containerSize.x / 2;
            bool isOverSizeLeft = objectLocalPos.x - objectSize - GetSize().x < -containerSize.x / 2;

            if (isOverSizeUp && isOverSizeDown && isOverSizeLeft && isOverSizeDown
                || GetSize().x > containerSize.x
                || GetSize().y > containerSize.y)
            {
                return false;
            }

            bool isObjectPassTopBorder = objectLocalPos.y - objectSize > dialogueContainerRect.rect.size.y / 2;
            bool isObjectPassLeftBorder = objectLocalPos.x + objectSize < -dialogueContainerRect.rect.size.x / 2;
            bool isObjectPassRightBorder = objectLocalPos.x - objectSize > dialogueContainerRect.rect.size.x / 2;

            float minX = -containerSize.x / 2 + GetSize().x / 2;
            float maxX = containerSize.x / 2 - GetSize().x / 2;
            float minY = -containerSize.y / 2 + GetSize().y / 2;
            float maxY = containerSize.y / 2 - GetSize().y / 2;

            float pivotSize = ConfigManager.Instance.TextBoxConfig.TextBoxPivotConfig.PivotSize;

            if (!isOverSizeUp && !isObjectPassLeftBorder && !isObjectPassRightBorder)
            {
                isPivotOverlapObject = objectLocalPos.y + objectSize + pivotSize + GetSize().y > containerSize.y / 2;
                resultPos.y = objectLocalPos.y + objectSize + GetSize().y / 2;
                resultPos.y = Mathf.Max(isPivotOverlapObject ? resultPos.y : resultPos.y + pivotSize, minY);
                resultPos.x = Mathf.Clamp(objectLocalPos.x, minX, maxX);
                pivotType = PivotType.DOWN;
                return true;
            }

            if (!isOverSizeRight && !isObjectPassTopBorder)
            {
                isPivotOverlapObject = objectLocalPos.x + objectSize + pivotSize + GetSize().x > containerSize.x / 2;
                resultPos.x = objectLocalPos.x + objectSize + GetSize().x / 2;
                resultPos.x = Mathf.Max(isPivotOverlapObject ? resultPos.x : resultPos.x + pivotSize, minX);
                resultPos.y = Mathf.Clamp(objectLocalPos.y, minY, maxY);
                pivotType = PivotType.LEFT;
                return true;
            }

            if (!isOverSizeLeft && !isObjectPassTopBorder)
            {
                isPivotOverlapObject = objectLocalPos.x - objectSize - pivotSize - GetSize().x < -containerSize.x / 2;
                resultPos.x = objectLocalPos.x - objectSize - GetSize().x / 2;
                resultPos.x = Mathf.Min(isPivotOverlapObject ? resultPos.x : resultPos.x - pivotSize, maxX);
                resultPos.y = Mathf.Clamp(objectLocalPos.y, minY, maxY);
                pivotType = PivotType.RIGHT;
                return true;
            }

            if (!isOverSizeDown)
            {
                isPivotOverlapObject = objectLocalPos.y - objectSize - pivotSize - GetSize().y < -containerSize.y / 2;
                resultPos.y = objectLocalPos.y - objectSize - GetSize().y / 2;
                resultPos.y = Mathf.Min(isPivotOverlapObject ? resultPos.y : resultPos.y - pivotSize, maxY);
                resultPos.x = Mathf.Clamp(objectLocalPos.x, minX, maxX);
                pivotType = PivotType.UP;
                return true;
            }

            return false;
        }

        private void TrySetTextBoxPivot(Vector3 objectWorldPos, float objectSize, TextBoxType textBoxType, PivotType pivotType, bool isPivotOverlapObject)
        {
            Vector2 objectLocalPos = Vector2.zero;
            if (!UIUtils.ConvertWorldPosToLocalRectPos(objectWorldPos, m_rectTransform, out objectLocalPos))
            {
                return;
            }

            TextBoxPivotPositionConfig pivotPositionConfig = ConfigManager.Instance.TextBoxConfig.TextBoxPivotConfig.GetTextBoxPivotPositionConfig(textBoxType, pivotType);
            m_textBoxPivot.rotation = Quaternion.Euler(0, 0, pivotPositionConfig.DegreeZ);
            m_textBoxPivot.anchorMax = pivotPositionConfig.AnchorMax;
            m_textBoxPivot.anchorMin = pivotPositionConfig.AnchorMin;
            float padding = ConfigManager.Instance.TextBoxConfig.TextBoxPivotConfig.Padding;
            Vector2 clampLocalPos = new Vector2(
                Mathf.Clamp(objectLocalPos.x, -GetSize().x / 2 + padding, GetSize().x / 2 - padding),
                Mathf.Clamp(objectLocalPos.y, -GetSize().y / 2 + padding, GetSize().y / 2 - padding)
                );

            bool isActive = true;
            switch (pivotType)
            {
                case PivotType.DOWN:
                case PivotType.UP:
                    if (isPivotOverlapObject)
                    {
                        if (GetSize().x - objectSize < ConfigManager.Instance.TextBoxConfig.TextBoxPivotConfig.PivotSize) //mean not have enough size for text box pivot
                        {
                            isActive = false;
                            break;
                        }
                        bool isObjectNearLeft = objectLocalPos.x < 0;
                        clampLocalPos.x = Mathf.Clamp(isObjectNearLeft ? objectLocalPos.x + objectSize * 1.2f : objectLocalPos.x - objectSize * 1.2f, -GetSize().x / 2, GetSize().x / 2);
                        m_textBoxPivot.rotation = Quaternion.Euler(0, isObjectNearLeft ? 0 : 180, m_textBoxPivot.rotation.eulerAngles.z); //flip the lean sprite if need
                    }
                    m_textBoxPivot.anchoredPosition = new Vector2(clampLocalPos.x, pivotPositionConfig.AnchorPos.y);
                    break;

                case PivotType.LEFT:
                case PivotType.RIGHT:
                    if (isPivotOverlapObject)
                    {
                        if (GetSize().y - objectSize < ConfigManager.Instance.TextBoxConfig.TextBoxPivotConfig.PivotSize) //mean not have enough size for text box pivot
                        {
                            isActive = false;
                            break;
                        }
                        bool isObjectNearDown = objectLocalPos.x < 0;
                        clampLocalPos.y = Mathf.Clamp(isObjectNearDown ? objectLocalPos.y + objectSize * 1.2f : objectLocalPos.y - objectSize * 1.2f, -GetSize().y / 2, GetSize().y / 2);
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
            //bool isPivotOverlapObject = objectLocalPos.x + objectSize / 2 < pivotRectPos.x + ConfigManager.Instance.TextBoxConfig.TextBoxPivotConfig.PivotSize / 2
            //    && objectLocalPos.x - objectSize / 2 > pivotRectPos.x - ConfigManager.Instance.TextBoxConfig.TextBoxPivotConfig.PivotSize / 2
            //    && objectLocalPos.y + objectSize / 2 < pivotRectPos.y + ConfigManager.Instance.TextBoxConfig.TextBoxPivotConfig.PivotSize / 2
            //    && objectLocalPos.y - objectSize / 2 > pivotRectPos.y - ConfigManager.Instance.TextBoxConfig.TextBoxPivotConfig.PivotSize / 2;
        }

        public Vector2 GetSize()
        {
            return m_rectTransform.rect.size;
        }
    }
}
