using UnityEngine;
using UnityEngine.EventSystems;

namespace GDC.Utils
{
    public class UIUtils : MonoBehaviour
    {
        public static void LockInput()
        {
            EventSystem e = FindObjectOfType<EventSystem>();
            if (e != null)  
            {
                e.enabled = false;
            }
        }
        public static void UnlockInput()
        {
            EventSystem e = FindObjectOfType<EventSystem>();
            if (e != null)  
            {
                e.enabled = true;
            }   
        }

        public static bool ConvertWorldPosToLocalRectPos(Vector3 worldPos, RectTransform parentRect, out Vector2 localPos)
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

        public static void ClearAllChild(Transform parent)
        {
            foreach (Transform child in parent)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
    }
}
