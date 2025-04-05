using System;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem
{
    public enum TextBoxType
    {
        NORMAL,
        LOUD,
        THINKING,
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
    public class TextBoxPivotPositionConfig
    {
        public TextBoxType TextBoxType;
        public PivotType PivotType;
        public float DegreeZ;
        public Vector2 AnchorPos;
        public Vector2 AnchorMax; //It's for anchor (neo) in UI canvas
        public Vector2 AnchorMin;
    }

    [Serializable]
    public class TextBoxPivotConfig
    {
        public List<TextBoxPivotPositionConfig> TextBoxPivotPositionConfigs;
        public float pivotSize;
        public Sprite NormalSprite;
        public Sprite LeanSprite;

        public TextBoxPivotPositionConfig GetTextBoxPivotPositionConfig(TextBoxType textBoxType, PivotType pivotType)
        {
            foreach (var config in TextBoxPivotPositionConfigs)
            {
                if (config.PivotType == pivotType && config.TextBoxType == textBoxType)
                {
                    return config;
                }
            }
            return null;
        }
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
        [SerializeField] private TextBoxPivotConfig textBoxPivotConfig;
        public TextBoxPivotConfig TextBoxPivotConfig { get; private set; }

        [Header("Sort ascending by minTextLength")]
        [SerializeField] private List<PreferHorizontalConfig> preferHorizontalConfigs;
        public List<PreferHorizontalConfig> PreferHorizontalConfigs { get; private set; }

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
    }
}
