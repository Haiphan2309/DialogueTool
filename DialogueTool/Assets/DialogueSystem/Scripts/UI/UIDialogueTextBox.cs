using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UIDialogueTextBox : MonoBehaviour
{
    private RectTransform rectTransform;
    [SerializeField] private RectTransform textBoxPivot;
    [SerializeField] private TMP_Text backText, frontText;

    [SerializeField] private float objectSize = 300;

#if UNITY_EDITOR
    [SerializeField] private Transform objectTrans;
    //[Button]
    //public void TestSetup()
    //{
    //    Setup(objectTrans.position);
    //}

    private void Update()
    {
        Setup(objectTrans.position);
        frontText.text = backText.text;
    }
#endif

    public void Setup(Vector3 objectWorldPos)
    {
        rectTransform = GetComponent<RectTransform>();

        frontText.text = backText.text;

        Vector2 resultPos = Vector2.zero;
        if (CheckPosition(objectWorldPos, out resultPos))
        {
            rectTransform.anchoredPosition = resultPos;
            return;
        }

        Debug.LogError("Text box is oversize!");
    }

    private bool CheckPosition(Vector3 objectWorldPos, out Vector2 resultPos)
    {
        RectTransform dialogueContainerRect = transform.parent.GetComponent<RectTransform>();
        Canvas canvas = GetComponentInParent<Canvas>();
        resultPos = Vector2.zero;
        if (rectTransform == null || dialogueContainerRect == null || canvas == null)
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

        Vector2 containerSize = dialogueContainerRect.rect.size;
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

        float minX = -containerSize.x / 2 + GetSize().x / 2;
        float maxX = containerSize.x / 2 - GetSize().x / 2;
        float minY = -containerSize.y / 2 + GetSize().y / 2;
        float maxY = containerSize.y / 2 - GetSize().y / 2;

        if (!isOverSizeUp) 
        {
            resultPos.y = Mathf.Max(objectLocalPos.y + objectSize + GetSize().y / 2, minY);
            resultPos.x = Mathf.Clamp(objectLocalPos.x, minX, maxX);
            return true;
        }
        
        if (!isOverSizeRight)
        {
            resultPos.x = Mathf.Max(objectLocalPos.x + objectSize + GetSize().x / 2, minX);
            //resultPos.y = Mathf.Clamp(objectLocalPos.y, minY, maxY);
            resultPos.y = maxY;
            return true;
        }

        if (!isOverSizeLeft)
        {
            resultPos.x = Mathf.Min(objectLocalPos.x - objectSize - GetSize().x / 2, maxX);
            //resultPos.y = Mathf.Clamp(objectLocalPos.y, minY, maxY);
            resultPos.y = maxY;
            return true;
        }

        if (!isOverSizeDown)
        {
            resultPos.y = Mathf.Min(objectLocalPos.y - objectSize - GetSize().y / 2, maxY);
            resultPos.x = Mathf.Clamp(objectLocalPos.x, minX, maxX);
            return true;
        }

        return false;
    }

    private Vector2 GetSize()
    {
        return rectTransform.rect.size;
    }
}
