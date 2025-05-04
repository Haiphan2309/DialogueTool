using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DSGroup : Group
{
    public void Setup(string titleValue, Vector2 position)
    {
        title = titleValue;
        SetPosition(new Rect(position, Vector2.zero));
    }

    public void Draw()
    {

    }
}
