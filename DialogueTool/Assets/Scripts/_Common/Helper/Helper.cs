using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Helpers
{
    public static class Helper
    {
        private static Camera _camera;
        public static Camera Camera
        {
            get {
                if (_camera == null) _camera = Camera.main;
                return _camera;
            }
        }
        public static List<T> ShuffleValue<T>(List<T> ts)
        {
            List<T> result = new List<T>(ts);
            var count = result.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i)
            {
                var r = UnityEngine.Random.Range(i, count);
                var tmp = result[i];
                result[i] = result[r];
                result[r] = tmp;
            }
            return result;
        }
        public static void ShuffleRef<T>(List<T> ts)
        {
            var count = ts.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i)
            {
                var r = UnityEngine.Random.Range(i, count);
                var tmp = ts[i];
                ts[i] = ts[r];
                ts[r] = tmp;
            }
        }

        private static readonly Dictionary<float, WaitForSeconds> WaitDictionary = new Dictionary<float, WaitForSeconds>();
        public static WaitForSeconds GetWait(float time)
        {
            if (WaitDictionary.TryGetValue(time, out var wait)) return wait;
            WaitDictionary[time] = new WaitForSeconds(time);
            return WaitDictionary[time];
        }

        private static PointerEventData _eventDataCurrentPosition;
        private static List<RaycastResult> _results;
        public static bool isOverUI()
        {
            _eventDataCurrentPosition = new PointerEventData(EventSystem.current) {position = Input.mousePosition};
            _results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(_eventDataCurrentPosition, _results);
            return _results.Count > 0;
        }

        public static Vector2 GetWorldPositionOfCanvasElement(RectTransform element)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(element, element.position, Camera, out var result);
            return result;
        }

        public static void DeletChildren(this Transform t)
        {
            foreach(Transform child in t) Object.Destroy(child.gameObject);
        }
    }
}
