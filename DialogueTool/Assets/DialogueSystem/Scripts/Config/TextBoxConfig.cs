using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PivotType
{
    NONE,
    UP,
    DOWN, 
    LEFT, 
    RIGHT,
}
[Serializable]
public class TextBoxPivotConfig
{
    public PivotType PivotType;
    public float DegreeZ;
    public Vector2 AnchorPos;
}

[Serializable]
[CreateAssetMenu(menuName = "Config/DialogueSystem/TextBoxConfig")]
public class TextBoxConfig : ScriptableObject
{
    public List<TextBoxPivotConfig> TextBoxPivotConfigs;
}
