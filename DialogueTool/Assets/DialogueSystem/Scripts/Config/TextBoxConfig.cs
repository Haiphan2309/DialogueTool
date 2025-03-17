using System;
using System.Collections.Generic;
using UnityEngine;

public enum TextBoxType
{
    NORMAL,
    LOUD,
}
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
    public TextBoxType TextBoxType;
    public PivotType PivotType;
    public float DegreeZ;
    public Vector2 AnchorPos;
}

[Serializable]
public class PreferHorizontalConfig
{
    public int MinTextLength;
    public float HorizontalSize;
}

[Serializable]
[CreateAssetMenu(menuName = "Config/DialogueSystem/TextBoxConfig")]
public class TextBoxConfig : ScriptableObject
{
    public Vector2 MinSize;
    public Vector2 MaxSize;
    public float PivotSize;
    public List<TextBoxPivotConfig> TextBoxPivotConfigs;

    [Header("Sort ascending by minTextLength")]
    [SerializeField] private List<PreferHorizontalConfig> preferHorizontalConfigs;

    public float GetPreferHorizontalSize(string text)
    {
        float result = MinSize.x;
        foreach (var config in preferHorizontalConfigs)
        {
            if (text.Length > config.MinTextLength)
            {
                result = config.MinTextLength;
            }
            else
            {
                break;
            }
        }
        return result;
    }    

    public List<PreferHorizontalConfig> GetPreferHorizontalConfigs()
    {
        return preferHorizontalConfigs;
    }
}
