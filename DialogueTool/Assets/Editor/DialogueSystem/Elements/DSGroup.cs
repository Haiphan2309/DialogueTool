using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DSGroup : Group
{
    public void Setup(Vector2 position)
    {
        SetPosition(new Rect(position, Vector2.zero));
    }

    public void Draw()
    {

    }
}
