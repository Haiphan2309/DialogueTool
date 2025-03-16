using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;

public static class GameUtils
{
    public static GameObject GetUIObjectUnderPointer()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);

        if (results.Count > 0)
        {
            return results[0].gameObject;
        }

        return null;
    }
    public static bool CompareVector3(Vector3 v1, Vector3 v2)
    {
        return (Mathf.RoundToInt(v1.x) == Mathf.RoundToInt(v2.x)
             && Mathf.RoundToInt(v1.y) == Mathf.RoundToInt(v2.y)
             && Mathf.RoundToInt(v1.z) == Mathf.RoundToInt(v2.z));
    }

    public static Vector3Int ConvertToVector3Int(Vector3 v)
    {
        return new Vector3Int((int)Mathf.Round(v.x), (int)Mathf.Round(v.y), (int)Mathf.Round(v.z));
    }
    /// <summary>
    /// Return the position snap to grid 
    /// </summary>
    /// <param name="position">Vector3 position</param>
    public static Vector3 SnapToGrid(Vector3 position)
    {
        return new Vector3(Mathf.Round(position.x), Mathf.Round(position.y), Mathf.Round(position.z));
    }
}
