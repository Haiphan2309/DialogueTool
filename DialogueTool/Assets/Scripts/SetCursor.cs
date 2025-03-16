using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCursor : MonoBehaviour
{
    [SerializeField] Texture2D cursorTexture;
    [SerializeField] Vector2 hotSpot;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.ForceSoftware);
    }
}
