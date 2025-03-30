using System;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem
{
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
        public float PivotSize;
        public Vector2 AnchorPos;
        public Vector2 AnchorMax;
        public Vector2 AnchorMin;
    }

    [Serializable]
    public class PreferHorizontalConfig //Using for prefer custom size of textbox, not dynamic follow text length
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
        [SerializeField] private List<TextBoxPivotConfig> TextBoxPivotConfigs;

        [Header("Sort ascending by minTextLength")]
        [SerializeField] private List<PreferHorizontalConfig> preferHorizontalConfigs;

        public TextBoxPivotConfig GetTextBoxPivotConfig(TextBoxType textBoxType, PivotType pivotType)
        {
            foreach(var config in TextBoxPivotConfigs)
            {
                if (config.PivotType == pivotType && config.TextBoxType == textBoxType)
                {
                    return config;
                }
            }
            return null;
        }
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
}
