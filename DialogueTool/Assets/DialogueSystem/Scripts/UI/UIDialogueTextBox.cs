using GDC.Managers;
using GDC.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem
{
    public class UIDialogueTextBox : MonoBehaviour
    {
        private RectTransform _rectTransform;
        [SerializeField] private RectTransform _textBoxArrow;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Image _background;

        private TextBoxType _textBoxType;

        public void Setup(string text, TextBoxType textBoxType, Vector3 objectWorldPos, float objectSize)
        {
            _rectTransform = GetComponent<RectTransform>();
            _textBoxType = textBoxType;

            _background.sprite = ConfigManager.Instance.TextBoxConfig.GetTextBoxSpriteConfig(textBoxType).TextBoxSprite;
            _background.type = Image.Type.Tiled;

            SetText(text);

            Vector2 resultPos = Vector2.zero;
            ArrowType arrowType = ArrowType.NONE;
            bool isArrowOverlapObject = false;
            if (TrySetPosition(objectWorldPos, objectSize, out resultPos, ref arrowType, ref isArrowOverlapObject))
            {
                _rectTransform.anchoredPosition = resultPos;
                TrySetTextBoxArrow(objectWorldPos, objectSize, textBoxType, arrowType, isArrowOverlapObject);
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
            ArrowType arrowType = ArrowType.NONE;
            bool isArrowOverlapObject = false;
            if (TrySetPosition(objectWorldPos, objectSize, out resultPos, ref arrowType, ref isArrowOverlapObject))
            {
                _rectTransform.anchoredPosition = resultPos;
                TrySetTextBoxArrow(objectWorldPos, objectSize, _textBoxType, arrowType, isArrowOverlapObject);
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
            _text.text = text;
        }

        private bool TrySetPosition(Vector3 objectWorldPos, float objectSize, out Vector2 resultPos, ref ArrowType arrowType, ref bool isArrowOverlapObject)
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

            float arrowSize = ConfigManager.Instance.TextBoxConfig.TextBoxArrowConfig.ArrowSize;

            if (!isOverSizeUp && !isObjectPassLeftBorder && !isObjectPassRightBorder)
            {
                isArrowOverlapObject = objectLocalPos.y + objectSize + arrowSize + GetSize().y > containerSize.y / 2;
                resultPos.y = objectLocalPos.y + objectSize + GetSize().y / 2;
                resultPos.y = Mathf.Max(isArrowOverlapObject ? resultPos.y : resultPos.y + arrowSize, minY);
                resultPos.x = Mathf.Clamp(objectLocalPos.x, minX, maxX);
                arrowType = ArrowType.DOWN;
                return true;
            }

            if (!isOverSizeRight && !isObjectPassTopBorder)
            {
                isArrowOverlapObject = objectLocalPos.x + objectSize + arrowSize + GetSize().x > containerSize.x / 2;
                resultPos.x = objectLocalPos.x + objectSize + GetSize().x / 2;
                resultPos.x = Mathf.Max(isArrowOverlapObject ? resultPos.x : resultPos.x + arrowSize, minX);
                resultPos.y = Mathf.Clamp(objectLocalPos.y, minY, maxY);
                arrowType = ArrowType.LEFT;
                return true;
            }

            if (!isOverSizeLeft && !isObjectPassTopBorder)
            {
                isArrowOverlapObject = objectLocalPos.x - objectSize - arrowSize - GetSize().x < -containerSize.x / 2;
                resultPos.x = objectLocalPos.x - objectSize - GetSize().x / 2;
                resultPos.x = Mathf.Min(isArrowOverlapObject ? resultPos.x : resultPos.x - arrowSize, maxX);
                resultPos.y = Mathf.Clamp(objectLocalPos.y, minY, maxY);
                arrowType = ArrowType.RIGHT;
                return true;
            }

            if (!isOverSizeDown)
            {
                isArrowOverlapObject = objectLocalPos.y - objectSize - arrowSize - GetSize().y < -containerSize.y / 2;
                resultPos.y = objectLocalPos.y - objectSize - GetSize().y / 2;
                resultPos.y = Mathf.Min(isArrowOverlapObject ? resultPos.y : resultPos.y - arrowSize, maxY);
                resultPos.x = Mathf.Clamp(objectLocalPos.x, minX, maxX);
                arrowType = ArrowType.UP;
                return true;
            }

            return false;
        }

        private void TrySetTextBoxArrow(Vector3 objectWorldPos, float objectSize, TextBoxType textBoxType, ArrowType arrowType, bool isArrowOverlapObject)
        {
            Vector2 objectLocalPos = Vector2.zero;
            if (!UIUtils.ConvertWorldPosToLocalRectPos(objectWorldPos, _rectTransform, out objectLocalPos))
            {
                return;
            }

            TextBoxArrowPositionConfig arrowPositionConfig = ConfigManager.Instance.TextBoxConfig.TextBoxArrowConfig.GetArrowPositionConfig(textBoxType, arrowType);
            _textBoxArrow.rotation = Quaternion.Euler(0, 0, arrowPositionConfig.DegreeZ);
            _textBoxArrow.anchorMax = arrowPositionConfig.AnchorMax;
            _textBoxArrow.anchorMin = arrowPositionConfig.AnchorMin;
            float paddingBorder = ConfigManager.Instance.TextBoxConfig.TextBoxArrowConfig.PaddingBorder;
            float mutiplePaddingLeanObject = ConfigManager.Instance.TextBoxConfig.TextBoxArrowConfig.MutiplePaddingLeanObject;
            Vector2 clampLocalPos = new Vector2(
                Mathf.Clamp(objectLocalPos.x, -GetSize().x / 2 + paddingBorder, GetSize().x / 2 - paddingBorder),
                Mathf.Clamp(objectLocalPos.y, -GetSize().y / 2 + paddingBorder, GetSize().y / 2 - paddingBorder)
                );

            bool isActive = true;
            switch (arrowType)
            {
                case ArrowType.DOWN:
                case ArrowType.UP:
                    if (isArrowOverlapObject)
                    {
                        if (GetSize().x - objectSize * mutiplePaddingLeanObject < ConfigManager.Instance.TextBoxConfig.TextBoxArrowConfig.ArrowSize) //mean not have enough size for text box arrow
                        {
                            isActive = false;
                            break;
                        }
                        bool isObjectNearLeft = objectLocalPos.x < 0;
                        clampLocalPos.x = Mathf.Clamp(
                            isObjectNearLeft ? objectLocalPos.x + objectSize * mutiplePaddingLeanObject : objectLocalPos.x - objectSize * mutiplePaddingLeanObject, 
                            -GetSize().x / 2 + paddingBorder, 
                            GetSize().x / 2 - paddingBorder);
                        _textBoxArrow.rotation = Quaternion.Euler(0, isObjectNearLeft ? 0 : 180, _textBoxArrow.rotation.eulerAngles.z); //flip the lean sprite if need
                    }
                    _textBoxArrow.anchoredPosition = new Vector2(clampLocalPos.x, arrowPositionConfig.AnchorPos.y);
                    break;

                case ArrowType.LEFT:
                case ArrowType.RIGHT:
                    if (isArrowOverlapObject)
                    {
                        if (GetSize().y - objectSize * mutiplePaddingLeanObject < ConfigManager.Instance.TextBoxConfig.TextBoxArrowConfig.ArrowSize) //mean not have enough size for text box arrow
                        {
                            isActive = false;
                            break;
                        }
                        bool isObjectNearDown = objectLocalPos.x < 0;
                        clampLocalPos.y = Mathf.Clamp(
                            isObjectNearDown ? objectLocalPos.y + objectSize * mutiplePaddingLeanObject : objectLocalPos.y - objectSize * mutiplePaddingLeanObject, 
                            -GetSize().y / 2 + paddingBorder, 
                            GetSize().y / 2 - paddingBorder);
                        _textBoxArrow.rotation = Quaternion.Euler(0, isObjectNearDown ? 0 : 180, _textBoxArrow.rotation.eulerAngles.z); //flip the lean sprite if need
                    }
                    _textBoxArrow.anchoredPosition = new Vector2(arrowPositionConfig.AnchorPos.x, clampLocalPos.y);
                    break;

                default:
                    isActive = false;
                    break;
            }

            _textBoxArrow.gameObject.SetActive(isActive);

            TextBoxArrowSpriteConfig spriteConfig = ConfigManager.Instance.TextBoxConfig.TextBoxArrowConfig.GetArrowSpriteConfig(textBoxType);
            _textBoxArrow.GetComponent<Image>().sprite = isArrowOverlapObject ? spriteConfig.LeanSprite : spriteConfig.NormalSprite;
        }

        public Vector2 GetSize()
        {
            return _rectTransform.rect.size;
        }
    }
}
